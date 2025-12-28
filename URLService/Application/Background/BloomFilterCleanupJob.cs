using Hangfire;
using Infrastructure.Persistence.Cache;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Background
{
  public class BloomFilterCleanupJob(
    ILogger<BloomFilterCleanupJob> logger,
    IOptions<BackgroundJobSettings> settings,
    ICacheService cacheService
  ) : BaseRecurringJob(logger)
  {
    public override JobConfiguration Configuration =>
      settings.Value.Jobs[nameof(BloomFilterCleanupJob)];

    public override void Enqueue()
    {
      BackgroundJob.Enqueue<BloomFilterCleanupJob>(x => x.ExecuteAsync(Configuration.JobName));
    }

    public override void Recurring()
    {
      RecurringJob.AddOrUpdate<BloomFilterCleanupJob>(
        nameof(BloomFilterCleanupJob),
        x => x.ExecuteAsync(Configuration.JobName),
        Configuration.CronExpression,
        new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc }
      );
    }

    protected override Task ExecuteInternalAsync(CancellationToken cancellationToken = default)
    {
      cacheService.CleanupNegativeCache();
      return Task.CompletedTask;
    }
  }
}
