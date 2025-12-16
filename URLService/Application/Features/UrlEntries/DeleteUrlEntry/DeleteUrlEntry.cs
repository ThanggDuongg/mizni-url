using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Cache;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UrlEntries.DeleteUrlEntry
{
  public sealed record DeleteUrlEntryCommand(string Code);

  public static class DeleteUrlEntryCommandHandler
  {
    public static async Task Handle(
      DeleteUrlEntryCommand command,
      IUrlContext urlContext,
      ICacheService cache,
      IHotKeyDetectionService hotKey,
      CancellationToken cancellationToken
    )
    {
      var code = command.Code.Trim();
      var entity =
        await urlContext
          .Get<UrlEntry>()
          .SingleOrDefaultAsync(e => e.Code == code, cancellationToken)
        ?? throw new UrlEntryNotFoundException(code);

      await urlContext.RemoveAsync(entity);
      await urlContext.SaveChangesAsync(cancellationToken);

      await cache.RemoveAsync(CacheKeyBuilder.Url(code), cancellationToken);
      await hotKey.ResetAsync(code);
    }
  }
}
