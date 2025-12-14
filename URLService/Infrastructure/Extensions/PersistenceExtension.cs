using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Cache;
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
        var mongoSettings = CreateMongoClientSettings(settings, logger);

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

      services.AddSingleton<IRedisConnection, RedisConnection>();
      services.AddSingleton<ICacheService, CacheService>();
      services.AddSingleton<IHotKeyDetectionService, HotKeyDetectionService>();

      return services;
    }

    private static MongoClientSettings CreateMongoClientSettings(
      MongoDbSettings settings,
      ILogger logger
    )
    {
      var mongoSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);

      mongoSettings.RetryWrites = true;
      mongoSettings.RetryReads = true;
      mongoSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(settings.CommandTimeout);
      mongoSettings.ClusterConfigurator = x =>
      {
        x.Subscribe<CommandStartedEvent>(e =>
        {
          LogCommandStarted(settings, logger, e);
        });

        x.Subscribe<CommandSucceededEvent>(e =>
        {
          LogCommandSucceeded(settings, logger, e);
        });

        x.Subscribe<CommandFailedEvent>(e =>
        {
          LogCommandFailed(logger, e);
        });
      };

      return mongoSettings;
    }

    private static void LogCommandStarted(
      MongoDbSettings settings,
      ILogger logger,
      CommandStartedEvent e
    )
    {
      if (settings.EnableSensitiveDataLogging && logger.IsEnabled(LogLevel.Debug))
      {
        logger.LogDebug(
          "MongoDB Command Started: {CommandName} - {Command}",
          e.CommandName,
          settings.EnableSensitiveDataLogging ? e.Command : null
        );
      }
    }

    private static void LogCommandSucceeded(
      MongoDbSettings settings,
      ILogger logger,
      CommandSucceededEvent e
    )
    {
      var duration = e.Duration.TotalMilliseconds;

      if (duration > settings.SlowQueryThresholdMs && logger.IsEnabled(LogLevel.Warning))
      {
        logger.LogWarning(
          "Slow Query Detected: {CommandName} took {Duration}ms (Threshold: {Threshold}ms) - RequestId: {RequestId}",
          e.CommandName,
          duration,
          settings.SlowQueryThresholdMs,
          e.RequestId
        );
      }
      else if (settings.EnableSensitiveDataLogging && logger.IsEnabled(LogLevel.Information))
      {
        logger.LogInformation(
          "MongoDB Command Succeeded: {CommandName} in {Duration}ms",
          e.CommandName,
          duration
        );
      }
    }

    private static void LogCommandFailed(ILogger logger, CommandFailedEvent e)
    {
      if (logger.IsEnabled(LogLevel.Error))
      {
        logger.LogError(
          e.Failure,
          "MongoDB Command Failed: {CommandName} - Duration: {Duration}ms",
          e.CommandName,
          e.Duration.TotalMilliseconds
        );
      }
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
        if (logger.IsEnabled(LogLevel.Error))
        {
          logger.LogError(
            ex,
            "Error ensuring database exists. Exception: {ExceptionMessage}",
            ex.Message
          );
        }
        throw new InvalidOperationException(
          "An error occurred while ensuring the database exists.",
          ex
        );
      }

      return host;
    }

    private static async Task SeedDataAsync(UrlContext context)
    {
      var demoUrl = $"https://leetcode.com/";

      var urlEntriesExists = await context.Get<UrlEntry>().AnyAsync();
      if (!urlEntriesExists)
      {
        var seedEntity = new UrlEntry
        {
          OriginalUrl = demoUrl,
          Code = "ltc",
          Expires = null,
        };

        var sequence = new Sequence { Name = nameof(UrlEntry), CurrentValue = 1 };

        await context.AddAsync(seedEntity);
        await context.AddAsync(sequence);
        await context.SaveChangesAsync();
      }
    }
  }
}
