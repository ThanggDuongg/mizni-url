using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure.Persistence
{
  public class PersistenceHealthCheck(IUrlContext context) : IHealthCheck
  {
    private readonly IUrlContext _context = context;

    public async Task<HealthCheckResult> CheckHealthAsync(
      HealthCheckContext context,
      CancellationToken cancellationToken = default
    )
    {
      try
      {
        var exists = await _context.IsDatabaseExistAsync(cancellationToken);

        return exists
          ? HealthCheckResult.Healthy("Database is connected")
          : HealthCheckResult.Degraded("Database does not exist");
      }
      catch (Exception ex)
      {
        return HealthCheckResult.Unhealthy("Database connection failed", ex);
      }
    }
  }
}
