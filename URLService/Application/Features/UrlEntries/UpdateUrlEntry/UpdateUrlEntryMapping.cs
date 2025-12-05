namespace Application.Features.UrlEntries.UpdateUrlEntry
{
  public static class UpdateUrlEntryMapping
  {
    public static UpdateUrlEntryCommand ToCommand(this UpdateUrlEntryRequestDto dto) =>
      new(dto.Code, dto.Expires, dto.Version);
  }
}
