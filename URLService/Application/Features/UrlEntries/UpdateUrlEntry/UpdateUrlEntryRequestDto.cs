namespace Application.Features.UrlEntries.UpdateUrlEntry
{
  public sealed record UpdateUrlEntryRequestDto(string Code, DateTime? Expires, long Version);
}
