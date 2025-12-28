namespace Shared.Configurations
{
  public sealed class RedisSettings
  {
    public string ConnectionString { get; set; } = string.Empty;

    public int DefaultDatabase { get; set; } = 0;

    public string InstanceName { get; set; } = string.Empty;
  }
}
