using Application.Features.Example.GetWeathers;
using Wolverine;

namespace WebAPI.Endpoints
{
  public static class ExampleEndpoints
  {
    public static void MapExampleEndpoints(this IEndpointRouteBuilder app)
    {
      var group = app.MapGroup("/example")
        .DisableAntiforgery()
        .AllowAnonymous()
        .WithTags("Example");

      group.MapPost("/", GetWeathers);
    }

    private static async Task<IResult> GetWeathers(WeatherRequestDto dto, IMessageBus bus)
    {
      var result = await bus.InvokeAsync<WeatherDataDto>(dto.ToQuery());
      return Results.Ok(result);
    }
  }
}
