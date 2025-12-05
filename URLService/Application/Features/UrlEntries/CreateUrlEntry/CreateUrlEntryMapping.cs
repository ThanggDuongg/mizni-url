using Domain.Entities;

namespace Application.Features.UrlEntries.CreateUrlEntry
{
  public static class CreateUrlEntryMapping
  {
    public static CreateUrlEntryCommand ToCommand(this CreateUrlEntryRequestDto dto) =>
      new(dto.OriginalUrl, dto.Expires);

    public static UrlEntry ToEntity(this CreateUrlEntryCommand command, string code) =>
      new()
      {
        OriginalUrl = command.OriginalUrl,
        Code = code,
        Expires = command.Expires,
      };

    public static CreateUrlEntryDataDto ToDto(this UrlEntry entity) =>
      new(entity.Id.ToString(), entity.Code, entity.OriginalUrl, entity.Expires);
  }
}
