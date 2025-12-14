using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Infrastructure.Persistence.Cache
{
  public class HotKeyDetectionService(
    IRedisConnection conn,
    IOptions<CacheTtlSettings> cacheTtlSettings
  ) : IHotKeyDetectionService
  {
    private const string LuaScript =
      @"
        local counter = redis.call('INCR', KEYS[1])
        if counter == 1 then
          redis.call('EXPIRE', KEYS[1], ARGV[1])
        end
        redis.call('ZINCRBY', KEYS[2], 1, ARGV[2])
        redis.call('EXPIRE', KEYS[2], ARGV[1])
        local ishot = 0
        if counter >= tonumber(ARGV[3]) then ishot = 1 end
        return {counter, ishot}
        ";

    public async Task<(long Count, bool IsHot)> IncrementAndCheckAsync(string code)
    {
      var db = conn.GetDatabase();
      var counterKey = CacheKeyBuilder.HotCounter(code);

      var result = (RedisResult[]?)
        await db.ScriptEvaluateAsync(
          LuaScript,
          keys: [counterKey, CacheKeyBuilder.HotZSet()],
          values:
          [
            cacheTtlSettings.Value.HotWindowSeconds,
            code,
            cacheTtlSettings.Value.HotThreshold,
          ]
        );

      if (result is null || result.Length < 2)
      {
        return (0, false);
      }

      long count = (long)result[0];
      bool isHot = (int)((long)result[1]) == 1;

      return (count, isHot);
    }

    public async Task<List<(string Code, double Score)>> GetTopAsync(int topN)
    {
      var db = conn.GetDatabase();
      var entries = await db.SortedSetRangeByScoreWithScoresAsync(
        CacheKeyBuilder.HotZSet(),
        order: Order.Descending,
        take: topN
      );

      return
      [
        .. entries.Select(e => ((e.Element.HasValue ? (string)e.Element! : string.Empty, e.Score))),
      ];
    }

    public Task ResetZsetAsync()
    {
      return conn.GetDatabase().KeyDeleteAsync(CacheKeyBuilder.HotZSet());
    }
  }
}
