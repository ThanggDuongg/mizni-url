namespace Shared.Configurations
{
  public sealed class BloomFilterSettings
  {
    public string Name { get; set; } = default!;

    public int ExpectedElements { get; set; } = 100000;

    public double ErrorRate { get; set; } = 0.01;
  }
}
