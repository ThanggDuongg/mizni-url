namespace Shared.Configurations
{
  public class RedisSettings
  {
    public string ConnectionString { get; set; } = string.Empty;

    public int DefaultDatabase { get; set; } = 0;

    public string InstanceName { get; set; } = string.Empty;
  }
}
