using Contracts.Cache;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StackExchange.Redis;

namespace Infrastructure.Persistence.Cache
{
  public class CacheService(
    HybridCache cache,
    IRedisConnection redis,
    IOptions<CacheTtlSettings> cacheTtlSettings,
    IOptions<CachePrewarmSettings> cachePrewarmSettings
  ) : ICacheService
  {
    public async Task<T?> GetOrCreateAsync<T>(
      string key,
      Func<CancellationToken, ValueTask<T?>> factory,
      CancellationToken cancellationToken = default
    )
      where T : ICacheValueModel
    {
      return await cache.GetOrCreateAsync(
        key: key,
        factory: factory,
        options: new HybridCacheEntryOptions
        {
          Expiration = TimeSpan.FromSeconds(cacheTtlSettings.Value.Normal.DistributedTtlSeconds),
          LocalCacheExpiration = TimeSpan.FromSeconds(
            cacheTtlSettings.Value.Normal.LocalTtlSeconds
          ),
        },
        cancellationToken: cancellationToken
      );
    }

    public async Task SetAsync<T>(
      T cacheValue,
      string key,
      bool isHot = false,
      CancellationToken cancellationToken = default
    )
      where T : ICacheValueModel
    {
      var ttl = isHot ? cacheTtlSettings.Value.Hot : cacheTtlSettings.Value.Normal;

      await cache.SetAsync(
        key: key,
        value: cacheValue,
        options: new HybridCacheEntryOptions
        {
          Expiration = TimeSpan.FromSeconds(ttl.DistributedTtlSeconds),
          LocalCacheExpiration = TimeSpan.FromSeconds(ttl.LocalTtlSeconds),
        },
        cancellationToken: cancellationToken
      );
    }

    public async Task SetBatchAsync<T>(
      IDictionary<string, T> entries,
      bool isHot,
      CancellationToken cancellationToken = default
    )
      where T : ICacheValueModel
    {
      var ttl = isHot ? cacheTtlSettings.Value.Hot : cacheTtlSettings.Value.Normal;

      var options = new HybridCacheEntryOptions
      {
        Expiration = TimeSpan.FromSeconds(ttl.DistributedTtlSeconds),
        LocalCacheExpiration = TimeSpan.FromSeconds(ttl.LocalTtlSeconds),
      };

      var batchSize = cachePrewarmSettings.Value.BatchSize;

      foreach (var batch in entries.Chunk(batchSize))
      {
        cancellationToken.ThrowIfCancellationRequested();

        var tasks = batch
          .Select(entry =>
            cache
              .SetAsync(entry.Key, entry.Value, options, cancellationToken: cancellationToken)
              .AsTask()
          )
          .ToList();

        await Task.WhenAll(tasks);
      }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
      await cache.RemoveAsync(key, cancellationToken);
    }

    public async Task<Dictionary<RedisKey, TimeSpan?>> GetTtlsAsync(
      IReadOnlyCollection<RedisKey> keys
    )
    {
      var db = redis.GetDatabase();
      var ttlTasks = keys.ToDictionary(key => key, key => db.KeyTimeToLiveAsync(key));
      await Task.WhenAll(ttlTasks.Values);

      return ttlTasks.ToDictionary(x => x.Key, x => x.Value.Result);
    }
  }
}
