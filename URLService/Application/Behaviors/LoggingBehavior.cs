using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Wolverine;

namespace Application.Behaviors
{
  public class ExecutionTracker
  {
    public Stopwatch Stopwatch { get; } = new Stopwatch();
    public string MessageType { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public IDisposable? LogContextScope { get; set; }
  }

  public static class LoggingBehavior
  {
    public const int WARNING_THRESHOLD_MS = 500;
    public const int CRITICAL_THRESHOLD_MS = 1000;

    public static ExecutionTracker Before(
      IMessageContext context,
      ILogger logger,
      IHttpContextAccessor accessor
    )
    {
      var messageType = context.Envelope?.Message?.GetType().Name ?? "Unknown";
      var httpContext = accessor.HttpContext;

      var correlationId =
        httpContext?.Items[Common.CORRELATION_ID_ITEM]?.ToString()
        ?? context.CorrelationId
        ?? Guid.NewGuid().ToString();
      context.CorrelationId = correlationId;

      if (logger.IsEnabled(LogLevel.Information))
      {
        logger.LogInformation(
          "START: {MessageType} | CorrelationId: {CorrelationId}",
          messageType,
          correlationId
        );
      }

      var tracker = new ExecutionTracker
      {
        MessageType = messageType,
        CorrelationId = correlationId,
        LogContextScope = LogContext.PushProperty(Common.CORRELATION_ID_ITEM, correlationId),
      };

      tracker.Stopwatch.Start();

      return tracker;
    }

    public static void After(ExecutionTracker tracker, ILogger logger)
    {
      tracker.Stopwatch.Stop();
      var elapsedMs = tracker.Stopwatch.ElapsedMilliseconds;

      if (elapsedMs > CRITICAL_THRESHOLD_MS)
      {
        if (logger.IsEnabled(LogLevel.Warning))
        {
          logger.LogWarning(
            "FINISH (CRITICAL): {MessageType} took {ElapsedMs}ms | CorrelationId: {CorrelationId}",
            tracker.MessageType,
            elapsedMs,
            tracker.CorrelationId
          );
        }
      }
      else if (elapsedMs > WARNING_THRESHOLD_MS)
      {
        if (logger.IsEnabled(LogLevel.Warning))
        {
          logger.LogWarning(
            "FINISH (SLOW): {MessageType} took {ElapsedMs}ms | CorrelationId: {CorrelationId}",
            tracker.MessageType,
            elapsedMs,
            tracker.CorrelationId
          );
        }
      }
      else
      {
        if (logger.IsEnabled(LogLevel.Information))
        {
          logger.LogInformation(
            "FINISH: {MessageType} took {ElapsedMs}ms | CorrelationId: {CorrelationId}",
            tracker.MessageType,
            elapsedMs,
            tracker.CorrelationId
          );
        }
      }
    }

    public static void Finally(ExecutionTracker tracker, ILogger logger)
    {
      if (tracker.Stopwatch.IsRunning)
      {
        tracker.Stopwatch.Stop();
        if (logger.IsEnabled(LogLevel.Error))
        {
          logger.LogError(
            "FAILED: {MessageType} took {ElapsedMs}ms | CorrelationId: {CorrelationId}",
            tracker.MessageType,
            tracker.Stopwatch.ElapsedMilliseconds,
            tracker.CorrelationId
          );
        }
      }

      tracker.LogContextScope?.Dispose();
    }
  }
}
