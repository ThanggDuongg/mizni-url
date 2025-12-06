using WebAPI.Endpoints;

namespace WebAPI.Extensions
{
  public static class ApiGroupExtension
  {
    public static IEndpointRouteBuilder MapApiGroups(this IEndpointRouteBuilder app)
    {
      var api = app.MapGroup($"{Common.API_VERSION}/api");

      api.MapExampleEndpoints();
      api.MapUrlEntryEndpoints();
      app.MapShortUrlRedirect();

      return app;
    }
  }
}
