using Application.Features.EntryUrl.CreateEntryUrl;
using Application.Features.EntryUrl.DeleteEntryUrl;
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
      group.MapDelete("/{code}", DeleteUrlEntry);
    }

    private static async Task<IResult> DeleteUrlEntry(
      string code,
      IMessageBus bus,
      CancellationToken cancellationToken = default
    )
    {
      await bus.InvokeAsync(new DeleteEntryUrlCommand(code), cancellationToken);
      return Results.NoContent();
    }

    private static async Task<IResult> CreateUrlEntry(
      CreateEntryUrlRequestDto dto,
      IMessageBus bus,
      CancellationToken cancellationToken = default
    )
    {
      var result = await bus.InvokeAsync<CreateEntryUrlDataDto>(dto.ToCommand(), cancellationToken);
      return Results.Ok(result);
    }
  }
}
