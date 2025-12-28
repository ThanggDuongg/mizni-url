namespace Contracts.UrlEntries.CreateUrlEntry
{
  public class CreateUrlEntryRequestDto
  {
    public string OriginalUrl { get; set; } = default!;

    public DateTime? ExpiredAt { get; set; }
  }
}
