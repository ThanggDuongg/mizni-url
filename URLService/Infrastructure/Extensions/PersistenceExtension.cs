using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

namespace Infrastructure.Extensions
{
  public static class PersistenceExtension
  {
    public static IServiceCollection AddPersistenceConfiguration(this IServiceCollection services)
    {
      services.AddSingleton<IMongoClient>(serviceProvider =>
      {
        var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
        var logger = serviceProvider.GetRequiredService<ILogger<UrlContext>>();
        var mongoSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);

        mongoSettings.RetryWrites = true;
        mongoSettings.RetryReads = true;
        mongoSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(settings.CommandTimeout);
        mongoSettings.ClusterConfigurator = cb =>
        {
          cb.Subscribe<CommandStartedEvent>(e =>
          {
            if (settings.EnableSensitiveDataLogging)
            {
              logger.LogDebug(
                "MongoDB Command Started: {CommandName} - {Command}",
                e.CommandName,
                e.Command
              );
            }
          });

          cb.Subscribe<CommandSucceededEvent>(e =>
          {
            var duration = e.Duration.TotalMilliseconds;

            if (duration > settings.SlowQueryThresholdMs)
            {
              logger.LogWarning(
                "Slow Query Detected: {CommandName} took {Duration}ms (Threshold: {Threshold}ms) - RequestId: {RequestId}",
                e.CommandName,
                duration,
                settings.SlowQueryThresholdMs,
                e.RequestId
              );
            }
            else if (settings.EnableSensitiveDataLogging)
            {
              logger.LogInformation(
                "MongoDB Command Succeeded: {CommandName} in {Duration}ms",
                e.CommandName,
                duration
              );
            }
          });

          cb.Subscribe<CommandFailedEvent>(e =>
          {
            logger.LogError(
              e.Failure,
              "MongoDB Command Failed: {CommandName} - Duration: {Duration}ms",
              e.CommandName,
              e.Duration.TotalMilliseconds
            );
          });
        };

        return new MongoClient(mongoSettings);
      });

      services.AddDbContext<UrlContext>(
        (serviceProvider, options) =>
        {
          var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
          var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
          options.UseMongoDB(mongoClient, settings.DatabaseName);

          if (settings.EnableSensitiveDataLogging)
          {
            options.EnableSensitiveDataLogging().EnableDetailedErrors();
          }
        }
      );

      services.AddScoped<IUrlContext, UrlContext>();

      return services;
    }

    public static async Task<IHost> EnsureDatabaseCreatedAsync(this IHost host)
    {
      using var scope = host.Services.CreateScope();
      var services = scope.ServiceProvider;
      var logger = services.GetRequiredService<ILogger<UrlContext>>();

      try
      {
        var context = services.GetRequiredService<UrlContext>();
        await SeedDataAsync(context);
      }
      catch (Exception ex)
      {
        logger.LogError(
          ex,
          "Error ensuring database exists. Exception: {ExceptionMessage}",
          ex.Message
        );
        throw new InvalidOperationException(
          "An error occurred while ensuring the database exists.",
          ex
        );
      }

      return host;
    }

    private static async Task SeedDataAsync(UrlContext context)
    {
      var collection = context.GetCollection<Domain.Entities.UrlEntry>();
      var demoUrl = $"https://leetcode.com/";
      var exists = await collection.Find(_ => true).AnyAsync();
      if (!exists)
      {
        var seed = new Domain.Entities.UrlEntry
        {
          OriginalUrl = demoUrl,
          Code = "ltc",
          Expires = null,
        };

        await collection.InsertOneAsync(seed);
      }
    }
  }
}
