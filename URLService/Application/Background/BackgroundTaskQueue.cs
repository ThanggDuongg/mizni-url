using System.Threading.Channels;

namespace Application.Background
{
  public class BackgroundTaskQueue : IBackgroundTaskQueue
  {
    private readonly Channel<Func<IServiceProvider, CancellationToken, ValueTask>> _queue;

    public BackgroundTaskQueue()
    {
      _queue = Channel.CreateUnbounded<Func<IServiceProvider, CancellationToken, ValueTask>>();
    }

    public ValueTask QueueAsync(
      Func<IServiceProvider, CancellationToken, ValueTask> work,
      CancellationToken cancellationToken = default
    ) => _queue.Writer.WriteAsync(work, cancellationToken);

    public ValueTask<Func<IServiceProvider, CancellationToken, ValueTask>> DequeueAsync(
      CancellationToken cancellationToken = default
    ) => _queue.Reader.ReadAsync(cancellationToken);
  }
}
