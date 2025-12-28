namespace Application.Background
{
  public interface IBackgroundJob
  {
    void Enqueue();

    void Recurring();

    Task ExecuteAsync(string jobName, CancellationToken cancellationToken = default);
  }
}
