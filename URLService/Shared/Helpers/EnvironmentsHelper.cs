namespace Shared.Helpers
{
  public static class EnvironmentsHelper
  {
    public static bool IsDevelopment()
    {
      return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
    }
  }
}
