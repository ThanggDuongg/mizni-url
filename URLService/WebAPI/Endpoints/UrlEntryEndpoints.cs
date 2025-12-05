using Application.Features.UrlEntries.CreateUrlEntry;
using Application.Features.UrlEntries.DeleteUrlEntry;
using Application.Features.UrlEntries.GetUrlEntry;
using Application.Features.UrlEntries.UpdateUrlEntry;
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
      group.MapPut("/", UpdateUrlEntry);
      group.MapDelete("/{code}", DeleteUrlEntry);
      group.MapGet("/{code}", GetUrlEntry);
    }

    private static async Task<IResult> UpdateUrlEntry(
      UpdateUrlEntryRequestDto dto,
      IMessageBus bus,
      CancellationToken cancellationToken = default
    )
    {
      await bus.InvokeAsync<bool>(dto.ToCommand(), cancellationToken);
      return Results.NoContent();
    }

    private static async Task<IResult> GetUrlEntry(
      string code,
      IMessageBus bus,
      CancellationToken cancellationToken = default
    )
    {
      var url = await bus.InvokeAsync<string?>(new GetUrlEntryQuery(code), cancellationToken);

      if (url is null)
      {
        return Results.NotFound();
      }

      return Results.Redirect(url, permanent: false);
    }

    private static async Task<IResult> DeleteUrlEntry(
      string code,
      IMessageBus bus,
      CancellationToken cancellationToken = default
    )
    {
      await bus.InvokeAsync(new DeleteUrlEntryCommand(code), cancellationToken);
      return Results.NoContent();
    }

    private static async Task<IResult> CreateUrlEntry(
      CreateUrlEntryRequestDto dto,
      IMessageBus bus,
      CancellationToken cancellationToken = default
    )
    {
      var result = await bus.InvokeAsync<CreateUrlEntryDataDto>(dto.ToCommand(), cancellationToken);
      return Results.Ok(result);
    }
  }
}
