namespace Application.Services.Interfaces
{
  public interface IUrlEntryService
  {
    Task<string?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
  }
}
