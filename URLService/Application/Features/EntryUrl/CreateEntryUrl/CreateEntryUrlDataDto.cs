namespace Application.Features.EntryUrl.CreateEntryUrl
{
  public sealed record CreateEntryUrlDataDto(
    string Id,
    string Code,
    string OriginalUrl,
    DateTime? Expires
  );
}
