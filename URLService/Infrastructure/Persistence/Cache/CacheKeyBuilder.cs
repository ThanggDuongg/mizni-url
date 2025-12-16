namespace Infrastructure.Persistence.Cache
{
  public static class CacheKeyBuilder
  {
    public static string Url(string code) => $"url:{code}";

    public static string HotCounter(string code) => $"url:hot:cnt:{code}";

    public static string HotZSet() => $"url:hot:zset";

    public const string GlobalLockPrefix = "lock:url:";
  }
}
