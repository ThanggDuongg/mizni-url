using Microsoft.AspNetCore.Antiforgery;

namespace WebAPI.Endpoints
{
  public static class AntiforgeryEndpoints
  {
    public static void MapAntiforgeryEndpoints(this IEndpointRouteBuilder app)
    {
      var group = app.MapGroup("/antiforgery")
        .DisableAntiforgery()
        .AllowAnonymous()
        .WithTags("Antiforgery");

      group.MapGet("/", GetAntiforgery);
    }

    private static IResult GetAntiforgery(IAntiforgery antiforgery, HttpContext context)
    {
      var tokens = antiforgery.GetAndStoreTokens(context);
      return Results.Ok(tokens.RequestToken);
    }
  }
}
