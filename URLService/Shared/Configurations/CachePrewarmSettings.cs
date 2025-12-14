namespace Shared.Configurations
{
  public sealed class CachePrewarmSettings
  {
    public int BatchSize { get; init; } = 50;

    public int IntervalSeconds { get; init; } = 60;

    public int TopN { get; init; } = 100;
  }
}
