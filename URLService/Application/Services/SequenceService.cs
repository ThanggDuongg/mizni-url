using Application.Services.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using MongoDB.Driver;

namespace Application.Services
{
  public class SequenceService(IUrlContext context) : ISequenceService
  {
    public async Task<long> GetNext(
      string sequenceName,
      CancellationToken cancellationToken = default
    )
    {
      var sequences = context.GetCollection<Sequence>();

      var existing = await sequences
        .Find(x => x.Name == sequenceName)
        .FirstOrDefaultAsync(cancellationToken);

      var version = existing?.Version ?? 0;

      var update = Builders<Sequence>
        .Update.Inc(x => x.CurrentValue, 1)
        .Set(x => x.Version, version + 1);
      var options = new FindOneAndUpdateOptions<Sequence>
      {
        IsUpsert = true,
        ReturnDocument = ReturnDocument.After,
      };

      var result = await sequences.FindOneAndUpdateAsync(
        filter: c => c.Name == sequenceName,
        update: update,
        options: options,
        cancellationToken: cancellationToken
      );

      return result.CurrentValue;
    }
  }
}
