namespace Application.Background
{
  public interface IBackgroundTaskQueue
  {
    ValueTask QueueAsync(
      Func<IServiceProvider, CancellationToken, ValueTask> work,
      CancellationToken cancellationToken = default
    );

    ValueTask<Func<IServiceProvider, CancellationToken, ValueTask>> DequeueAsync(
      CancellationToken cancellationToken = default
    );
  }
}
