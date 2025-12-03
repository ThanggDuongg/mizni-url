using Application.Features.EntryUrl.CreateEntryUrl;
using Wolverine;

namespace WebAPI.Endpoints
{
  public static class UrlEntryEndpoints
  {
    public static void MapUrlEntryEndpoints(this IEndpointRouteBuilder app)
    {
      var group = app.MapGroup("/url-entry")
        .DisableAntiforgery()
        .AllowAnonymous()
        .WithTags("UrlEntry");

      group.MapPost("/", CreateUrlEntry);
    }

    private static async Task<IResult> CreateUrlEntry(CreateEntryUrlRequestDto dto, IMessageBus bus)
    {
      var result = await bus.InvokeAsync<CreateEntryUrlDataDto>(dto.ToCommand());
      return Results.Ok(result);
    }
  }
}
