using System.Threading.RateLimiting;

namespace WebAPI.Extensions
{
  public static class RateLimitExtension
  {
    public static IServiceCollection AddRateLimiting(
      this IServiceCollection services,
      IWebHostEnvironment env
    )
    {
      services.AddRateLimiter(options =>
      {
        options.AddPolicy(
          Policy.RateLimitRedirect,
          context =>
          {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            if (ip.Contains(','))
            {
              ip = ip.Split(',')[0].Trim();
            }

            return RateLimitPartition.GetTokenBucketLimiter(
              ip,
              _ => new TokenBucketRateLimiterOptions
              {
                TokenLimit = 30, // burst
                TokensPerPeriod = 15, // 15 req/s
                ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                AutoReplenishment = true,
                QueueLimit = 0,
              }
            );
          }
        );

        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        if (env.IsDevelopment())
        {
          options.OnRejected = async (context, cancellationToken) =>
          {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

            if (logger.IsEnabled(LogLevel.Warning))
            {
              logger.LogWarning(
                "Rate limit exceeded: {IP} → {Path}",
                context.HttpContext.Connection.RemoteIpAddress,
                context.HttpContext.Request.Path
              );
            }
          };
        }
      });

      return services;
    }
  }
}
