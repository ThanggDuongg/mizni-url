namespace Shared.Configurations
{
  public class BloomFilterSettings
  {
    public string Name { get; set; } = default!;

    public int ExpectedElements { get; set; } = 100000;

    public double ErrorRate { get; set; } = 0.01;
  }
}
