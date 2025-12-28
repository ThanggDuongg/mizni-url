using Hangfire.Dashboard;

namespace Infrastructure.Filters
{
  public sealed class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
  {
    public bool Authorize(DashboardContext context)
    {
      return true;
    }
  }
}
