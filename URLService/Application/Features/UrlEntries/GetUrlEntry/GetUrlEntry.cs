using Application.Services.Interfaces;

namespace Application.Features.UrlEntries.GetUrlEntry
{
  public sealed record GetUrlEntryQuery(string Code);

  public static class GetUrlEntryQueryHandler
  {
    public static async Task<string?> Handle(
      GetUrlEntryQuery query,
      IUrlEntryService urlEntryService,
      CancellationToken cancellationToken
    )
    {
      return await urlEntryService.GetByCodeAsync(query.Code, cancellationToken);
    }
  }
}
