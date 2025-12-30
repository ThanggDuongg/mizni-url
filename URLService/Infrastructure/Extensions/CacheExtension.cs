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
        var config = BuildRedisConfigurationOptions(redisSettings);
        return ConnectionMultiplexer.Connect(config);
      });

      services.AddSingleton<IRedisConnection, RedisConnection>();

      services.AddMemoryCache();
      services.AddStackExchangeRedisCache(options =>
      {
        var config = BuildRedisConfigurationOptions(redisSettings);

        options.ConfigurationOptions = config;
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

    private static ConfigurationOptions BuildRedisConfigurationOptions(RedisSettings settings)
    {
#if DEBUG
      var config = ConfigurationOptions.Parse(settings.ConnectionString);
      config.AbortOnConnectFail = false;
      config.AllowAdmin = true;
      config.DefaultDatabase = settings.DefaultDatabase;
      return config;
#endif

      var uri = new Uri(settings.ConnectionString);

      return new ConfigurationOptions
      {
        AbortOnConnectFail = false,
        AllowAdmin = true,
        DefaultDatabase = settings.DefaultDatabase,
        Ssl = false,
        User = string.IsNullOrEmpty(uri.UserInfo) ? "default" : uri.UserInfo.Split(':')[0],
        Password = string.IsNullOrEmpty(uri.UserInfo) ? null : uri.UserInfo.Split(':')[1],
        EndPoints = { { uri.Host, uri.Port } },
      };
    }
  }
}
