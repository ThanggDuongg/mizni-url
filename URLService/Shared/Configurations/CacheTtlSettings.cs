namespace Shared.Configurations
{
  public sealed class CacheTtlSettings
  {
    public CacheDuration Normal { get; set; } = default!;

    public CacheDuration Hot { get; set; } = default!;

    public CacheDuration Negative { get; set; } = default!;

    public int HotWindowSeconds { get; set; }

    public int HotThreshold { get; set; }
  }

  public sealed class CacheDuration
  {
    public int DistributedTtlSeconds { get; set; }

    public int LocalTtlSeconds { get; set; }
  }
}
