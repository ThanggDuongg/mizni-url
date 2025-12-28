namespace Shared.Configurations
{
  public sealed class MongoDbSettings
  {
    public string ConnectionString { get; set; } = string.Empty;

    public string DatabaseName { get; set; } = string.Empty;

    public int SlowQueryThresholdMs { get; set; } = 5000;

    public bool EnableSensitiveDataLogging { get; set; } = false;

    public int CommandTimeout { get; set; } = 30;
  }
}
