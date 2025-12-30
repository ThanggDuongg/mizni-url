using Application.Background;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Infrastructure.Filters;
using MongoDB.Driver;

namespace WebAPI.Extensions
{
  public static class HangfireExtension
  {
    public static IServiceCollection AddHangfireInfrastructure(
      this IServiceCollection services,
      IConfiguration configuration
    )
    {
      var settings = configuration
        .GetRequiredSection(nameof(MongoDbSettings))
        .Get<MongoDbSettings>()!;

      services.AddHangfire(config =>
        config
          .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseMongoStorage(
            MongoClientSettings.FromConnectionString(settings.ConnectionString),
            settings.DatabaseName,
            new MongoStorageOptions
            {
              Prefix = "Hangfire",
              MigrationOptions = new MongoMigrationOptions
              {
                MigrationStrategy = new MigrateMongoMigrationStrategy(),
                BackupStrategy = new CollectionMongoBackupStrategy(),
              },
              CheckConnection = true,
              CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection,
              SlidingInvisibilityTimeout = TimeSpan.FromMinutes(60),
            }
          )
      );
      GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 3 });

      services.AddHangfireServer();
      services.AddScoped<IRecurringJob, BloomFilterCleanupJob>();
      services.AddScoped<IRecurringJob, DeleteExpiredUrlEntriesJob>();

      return services;
    }

    public static IApplicationBuilder UseHangfire(this IApplicationBuilder app)
    {
      app.UseHangfireDashboard(
        "/jobs",
        new DashboardOptions
        {
          Authorization = [new HangfireAuthorizationFilter()],
          IgnoreAntiforgeryToken = true,
        }
      );
      app.RegisterAllRecurringJobs();

      return app;
    }

    private static IApplicationBuilder RegisterAllRecurringJobs(this IApplicationBuilder app)
    {
      using var scope = app.ApplicationServices.CreateScope();

      var jobs = scope.ServiceProvider.GetServices<IRecurringJob>();

      foreach (var job in jobs)
      {
        job.Recurring();
      }

      return app;
    }
  }
}
