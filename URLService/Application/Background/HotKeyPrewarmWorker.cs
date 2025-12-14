using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Cache;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StackExchange.Redis;

namespace Application.Background
{
  public class HotKeyPrewarmWorker(
    IHotKeyDetectionService hot,
    IServiceScopeFactory scopeFactory,
    ICacheService cache,
    ILogger<HotKeyPrewarmWorker> logger,
    IOptions<CacheTtlSettings> cacheTtlSettings,
    IOptions<CachePrewarmSettings> cachePrewarmSettings
  ) : BackgroundService
  {
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      var hotTtl = TimeSpan.FromSeconds(cacheTtlSettings.Value.Hot.DistributedTtlSeconds);
      var prewarmThreshold = TimeSpan.FromTicks(hotTtl.Ticks / 3); // 33%

      while (!stoppingToken.IsCancellationRequested)
      {
        try
        {
          await ProcessAsync(prewarmThreshold, stoppingToken);
        }
        catch (Exception ex)
        {
          logger.LogError(ex, "HotKeyPrewarmWorker failed");
        }

        await Task.Delay(
          TimeSpan.FromSeconds(cachePrewarmSettings.Value.IntervalSeconds),
          stoppingToken
        );
      }
    }

    private async Task ProcessAsync(TimeSpan prewarmThreshold, CancellationToken stoppingToken)
    {
      using var scope = scopeFactory.CreateScope();
      var urlContext = scope.ServiceProvider.GetRequiredService<IUrlContext>();

      var top = await hot.GetTopAsync(cachePrewarmSettings.Value.TopN);
      var codes = top.Select(x => x.Code).ToList();
      if (codes.Count == 0)
      {
        return;
      }

      var keyToCode = top.ToDictionary(x => (RedisKey)CacheKeyBuilder.Url(x.Code), x => x.Code);
      var ttls = await cache.GetTtlsAsync(keyToCode.Keys);
      var needPrewarmCodes = ttls.Where(x => x.Value is null || x.Value < prewarmThreshold)
        .Select(x => keyToCode[x.Key])
        .ToList();

      if (needPrewarmCodes.Count == 0)
      {
        return;
      }

      var collection = urlContext.GetCollection<UrlEntry>();
      var entries = await collection
        .Find(x => needPrewarmCodes.Contains(x.Code))
        .ToListAsync(stoppingToken);

      if (entries.Count > 0)
      {
        var entriesDict = entries.ToDictionary(x => CacheKeyBuilder.Url(x.Code));
        await cache.SetBatchAsync(entriesDict, isHot: true, stoppingToken);

        if (logger.IsEnabled(LogLevel.Information))
        {
          logger.LogInformation(
            "Prewarmed {Count} hot keys (threshold={Threshold}s, topN={TopN})",
            entries.Count,
            prewarmThreshold.TotalSeconds,
            cachePrewarmSettings.Value.TopN
          );
        }
      }
    }
  }
}
