namespace Shared.Configurations
{
  public class CacheTtlSettings
  {
    public CacheDuration Normal { get; set; } = default!;

    public CacheDuration Hot { get; set; } = default!;

    public int HotWindowSeconds { get; set; }

    public int HotThreshold { get; set; }
  }

  public class CacheDuration
  {
    public int DistributedTtlSeconds { get; set; }

    public int LocalTtlSeconds { get; set; }
  }
}
