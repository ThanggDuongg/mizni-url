using Infrastructure.Persistence.Cache;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure.Extensions
{
  public static class CacheExtension
  {
    public static IServiceCollection AddCacheConfiguration(
      this IServiceCollection services,
      IConfiguration configuration
    )
    {
      var cacheTtlSettings = configuration
        .GetRequiredSection(nameof(CacheTtlSettings))
        .Get<CacheTtlSettings>()!;
      var redisSettings = configuration
        .GetRequiredSection(nameof(RedisSettings))
        .Get<RedisSettings>()!;

      services.AddSingleton<IConnectionMultiplexer>(_ =>
      {
        var config = ConfigurationOptions.Parse(redisSettings.ConnectionString);
        config.AbortOnConnectFail = false;
        config.AllowAdmin = true;
        config.DefaultDatabase = redisSettings.DefaultDatabase;

        return ConnectionMultiplexer.Connect(config);
      });

      services.AddSingleton<IRedisConnection, RedisConnection>();

      services.AddMemoryCache();
      services.AddStackExchangeRedisCache(options =>
      {
        options.Configuration = redisSettings.ConnectionString;
        options.InstanceName = redisSettings.InstanceName;
      });
      services.AddHybridCache(cacheOptions =>
      {
        cacheOptions.DefaultEntryOptions = new HybridCacheEntryOptions
        {
          Expiration = TimeSpan.FromMinutes(cacheTtlSettings.Normal.DistributedTtlSeconds),
          LocalCacheExpiration = TimeSpan.FromMinutes(cacheTtlSettings.Normal.LocalTtlSeconds),
        };
      });

      return services;
    }
  }
}
