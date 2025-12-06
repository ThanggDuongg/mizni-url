namespace Application.Features.UrlEntries.CreateUrlEntry
{
  public sealed record CreateUrlEntryDataDto(
    string Id,
    string Code,
    string OriginalUrl,
    DateTime? Expires
  );
}
