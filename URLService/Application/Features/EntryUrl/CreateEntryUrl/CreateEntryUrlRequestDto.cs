namespace Application.Features.EntryUrl.CreateEntryUrl
{
  public class CreateEntryUrlRequestDto
  {
    public string OriginalUrl { get; set; } = default!;

    public DateTime? Expires { get; set; }
  }
}
