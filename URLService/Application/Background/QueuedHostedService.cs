using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Background
{
  public class QueuedHostedService(
    IServiceProvider root,
    IBackgroundTaskQueue queue,
    ILogger<QueuedHostedService> logger
  ) : BackgroundService
  {
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        var work = await queue.DequeueAsync(stoppingToken);
        using var scope = root.CreateScope();

        try
        {
          await work(scope.ServiceProvider, stoppingToken);
        }
        catch (Exception ex)
        {
          if (logger.IsEnabled(LogLevel.Error))
          {
            logger.LogError(ex, "Background task failed: {Message}", ex.Message);
          }
        }
      }
    }
  }
}
