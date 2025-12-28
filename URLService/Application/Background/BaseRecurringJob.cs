using Hangfire;
using Microsoft.Extensions.Logging;

namespace Application.Background
{
  public abstract class BaseRecurringJob(ILogger logger) : IRecurringJob
  {
    public abstract JobConfiguration Configuration { get; }

    public abstract void Enqueue();

    public abstract void Recurring();

    [JobDisplayName("{0}")]
    public async Task ExecuteAsync(string jobName, CancellationToken cancellationToken = default)
    {
      if (!Configuration.Enabled)
      {
        return;
      }

      if (logger.IsEnabled(LogLevel.Information))
      {
        logger.LogInformation("Start {Job}", jobName);
      }

      try
      {
        if (logger.IsEnabled(LogLevel.Information))
        {
          logger.LogInformation("Running {Job}", jobName);
        }
        await ExecuteInternalAsync(cancellationToken);
        if (logger.IsEnabled(LogLevel.Information))
        {
          logger.LogInformation("Complete {Job}", jobName);
        }
      }
      catch (Exception ex)
      {
        if (logger.IsEnabled(LogLevel.Error))
        {
          logger.LogError(ex, "Error occurred during execution of {Job}", jobName);
        }
        throw;
      }
    }

    protected abstract Task ExecuteInternalAsync(CancellationToken cancellationToken = default);
  }
}
