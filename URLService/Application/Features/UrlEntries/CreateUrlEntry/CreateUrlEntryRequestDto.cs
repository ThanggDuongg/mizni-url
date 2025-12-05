namespace Application.Features.UrlEntries.CreateUrlEntry
{
  public class CreateUrlEntryRequestDto
  {
    public string OriginalUrl { get; set; } = default!;

    public DateTime? Expires { get; set; }
  }
}
