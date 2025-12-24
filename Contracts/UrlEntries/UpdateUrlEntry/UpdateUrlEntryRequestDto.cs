namespace Contracts.UrlEntries.UpdateUrlEntry
{
  public sealed record UpdateUrlEntryRequestDto(string Code, DateTime? Expires, long Version);
}
