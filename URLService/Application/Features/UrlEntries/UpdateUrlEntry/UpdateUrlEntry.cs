using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Cache;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UrlEntries.UpdateUrlEntry
{
  public sealed record UpdateUrlEntryCommand(string Code, DateTime? Expires, long Version);

  public static class UpdateUrlEntryCommandHandler
  {
    public static async Task Handle(
      UpdateUrlEntryCommand command,
      IUrlContext urlContext,
      ICacheService cache,
      CancellationToken cancellationToken
    )
    {
      var entry =
        await urlContext
          .Get<UrlEntry>()
          .SingleOrDefaultAsync(x => x.Code == command.Code, cancellationToken)
        ?? throw new UrlEntryNotFoundException(command.Code);

      if (entry.Expires == command.Expires)
      {
        return;
      }

      entry.Expires = command.Expires;
      entry.Version = command.Version;

      await urlContext.UpdateAsync(entry);
      await urlContext.SaveChangesAsync(cancellationToken);

      await cache.RemoveAsync(CacheKeyBuilder.Url(entry.Code), cancellationToken);
    }
  }
}
