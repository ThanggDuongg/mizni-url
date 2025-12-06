using Domain.Entities;
using Infrastructure.Persistence;
using MongoDB.Driver;

namespace Application.Features.UrlEntries.GetUrlEntry
{
  public sealed record GetUrlEntryQuery(string Code);

  public static class GetUrlEntryQueryHandler
  {
    public static async Task<string?> Handle(
      GetUrlEntryQuery query,
      IUrlContext urlContext,
      CancellationToken cancellationToken
    )
    {
      var collection = urlContext.GetCollection<UrlEntry>();

      var filter = Builders<UrlEntry>.Filter.Eq(x => x.Code, query.Code);

      var entry = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);

      if (entry is null || (entry.Expires is not null && entry.Expires < DateTime.UtcNow))
      {
        return null;
      }

      return entry.OriginalUrl;
    }
  }
}
