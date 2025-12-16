using Domain.Entities.Abstracts;
using StackExchange.Redis;

namespace Infrastructure.Persistence.Cache
{
  public interface ICacheService
  {
    Task<T?> GetOrCreateAsync<T>(
      string key,
      Func<CancellationToken, ValueTask<T>> factory,
      CancellationToken cancellationToken = default
    )
      where T : BaseEntity;

    Task SetAsync<T>(
      T entity,
      string key,
      bool isHot = false,
      CancellationToken cancellationToken = default
    )
      where T : BaseEntity;

    Task SetBatchAsync<T>(
      IDictionary<string, T> entries,
      bool isHot,
      CancellationToken cancellationToken = default
    )
      where T : BaseEntity;

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    Task<Dictionary<RedisKey, TimeSpan?>> GetTtlsAsync(IReadOnlyCollection<RedisKey> keys);
  }
}
