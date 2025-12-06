namespace Shared.Configurations
{
  public class CodeGeneratorOptions
  {
    public long Offset { get; set; }
    public long XorKey { get; set; }
    public string Salt { get; set; } = string.Empty;
    public int MinHashLength { get; set; }
  }
}
