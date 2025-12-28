namespace Shared.Configurations
{
  public sealed class BackgroundJobSettings
  {
    public Dictionary<string, JobConfiguration> Jobs { get; init; } = [];
  }

  public sealed class JobConfiguration
  {
    public string JobName { get; init; } = default!;

    public bool Enabled { get; init; } = true;

    public string CronExpression { get; init; } = default!;
  }
}
