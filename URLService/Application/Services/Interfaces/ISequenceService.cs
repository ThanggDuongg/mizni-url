namespace Application.Services.Interfaces
{
  public interface ISequenceService
  {
    Task<long> GetNext(string sequenceName, CancellationToken cancellationToken = default);
  }
}
