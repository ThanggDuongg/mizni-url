using Application.Background;
using Application.Services.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Cache;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Application.Services
{
  public class UrlEntryService(
    ICacheService urlEntryCacheService,
    IUrlContext urlContext,
    IBackgroundTaskQueue backgroundTaskQueue
  ) : IUrlEntryService
  {
    public async Task<string?> GetByCodeAsync(
      string code,
      CancellationToken cancellationToken = default
    )
    {
      var urlEntry = await urlEntryCacheService.GetOrCreateAsync(
        CacheKeyBuilder.Url(code),
        factory: async ct =>
        {
          var collection = urlContext.GetCollection<UrlEntry>();
          return await collection.Find(x => x.Code == code).FirstOrDefaultAsync(ct);
        },
        cancellationToken
      );
      if (urlEntry is null)
      {
        return null;
      }

      await backgroundTaskQueue.QueueAsync(
        async (sp, ct) =>
        {
          var hotKey = sp.GetRequiredService<IHotKeyDetectionService>();
          var (_, isHot) = await hotKey.IncrementAndCheckAsync(code);

          if (isHot)
          {
            await urlEntryCacheService.SetAsync(
              urlEntry,
              CacheKeyBuilder.Url(code),
              isHot: true,
              ct
            );
          }
        },
        cancellationToken
      );

      return urlEntry.OriginalUrl;
    }
  }
}
