using Domain.Entities;
using Hangfire;
using Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Application.Background
{
  public class DeleteExpiredUrlEntriesJob(
    ILogger<DeleteExpiredUrlEntriesJob> logger,
    IOptions<BackgroundJobSettings> settings,
    IUrlContext urlContext
  ) : BaseRecurringJob(logger)
  {
    public override JobConfiguration Configuration =>
      settings.Value.Jobs[nameof(DeleteExpiredUrlEntriesJob)];

    public override void Enqueue()
    {
      BackgroundJob.Enqueue<DeleteExpiredUrlEntriesJob>(x => x.ExecuteAsync(Configuration.JobName));
    }

    public override void Recurring()
    {
      RecurringJob.AddOrUpdate<DeleteExpiredUrlEntriesJob>(
        nameof(DeleteExpiredUrlEntriesJob),
        x => x.ExecuteAsync(Configuration.JobName),
        Configuration.CronExpression,
        new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc }
      );
    }

    protected override async Task ExecuteInternalAsync(
      CancellationToken cancellationToken = default
    )
    {
      var now = DateTime.UtcNow;
      var hardDeleteAfterDays = 30;

      var collection = urlContext.GetCollection<UrlEntry>();

      var softFilter =
        Builders<UrlEntry>.Filter.Eq(x => x.IsDeleted, false)
        & Builders<UrlEntry>.Filter.Lte(x => x.ExpiredAt, now);

      var softUpdate = Builders<UrlEntry>
        .Update.Set(x => x.IsDeleted, true)
        .Set(x => x.MarkedDeletedAt, now);

      var softResult = await collection.UpdateManyAsync(
        softFilter,
        softUpdate,
        cancellationToken: cancellationToken
      );
      if (logger.IsEnabled(LogLevel.Information))
      {
        logger.LogInformation("Soft deleted {Count} expired UrlEntries", softResult.ModifiedCount);
      }

      var hardDeleteBefore = now.AddDays(-hardDeleteAfterDays);
      var hardFilter =
        Builders<UrlEntry>.Filter.Eq(x => x.IsDeleted, true)
        & Builders<UrlEntry>.Filter.Lte(x => x.MarkedDeletedAt, hardDeleteBefore);

      var hardResult = await collection.DeleteManyAsync(hardFilter, cancellationToken);
      if (logger.IsEnabled(LogLevel.Information))
      {
        logger.LogInformation("Hard deleted {Count} UrlEntries", hardResult.DeletedCount);
      }
    }
  }
}
