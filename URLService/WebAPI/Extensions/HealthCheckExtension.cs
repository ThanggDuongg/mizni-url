using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace WebAPI.Extensions
{
  public static class HealthCheckExtension
  {
    public static IServiceCollection AddApplicationHealthChecks(this IServiceCollection services)
    {
      services.AddHealthChecks();
      return services;
    }

    public static IEndpointRouteBuilder MapApplicationHealthChecks(
      this IEndpointRouteBuilder endpoint
    )
    {
      endpoint.MapHealthChecks(
        "/health",
        new HealthCheckOptions
        {
          ResponseWriter = async (context, report) =>
          {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Health check status: {Status}", report.Status);

            foreach (var entry in report.Entries)
            {
              logger.LogDebug(
                "Health check {Name}: {Status} - {Duration}ms",
                entry.Key,
                entry.Value.Status,
                entry.Value.Duration.TotalMilliseconds
              );
            }

            await context.Response.WriteAsJsonAsync(
              new
              {
                status = report.Status.ToString(),
                checks = report.Entries.Select(e => new
                {
                  name = e.Key,
                  status = e.Value.Status.ToString(),
                  duration = e.Value.Duration.TotalMilliseconds,
                }),
              }
            );
          },
        }
      );
      return endpoint;
    }
  }
}
