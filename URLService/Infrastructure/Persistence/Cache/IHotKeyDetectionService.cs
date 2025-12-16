namespace Infrastructure.Persistence.Cache
{
  public interface IHotKeyDetectionService
  {
    Task<(long Count, bool IsHot)> IncrementAndCheckAsync(string code);

    Task<List<(string Code, double Score)>> GetTopAsync(int topN);

    Task ResetZsetAsync();

    Task ResetAsync(string code);

    Task ResetAllAsync();
  }
}
