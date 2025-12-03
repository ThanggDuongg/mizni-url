using Domain.Entities;

namespace Application.Features.EntryUrl.CreateEntryUrl
{
  public static class CreateEntryUrlMapping
  {
    public static CreateEntryUrlCommand ToCommand(this CreateEntryUrlRequestDto dto) =>
      new(dto.OriginalUrl, dto.Expires);

    public static UrlEntry ToEntity(this CreateEntryUrlCommand command, string code) =>
      new()
      {
        OriginalUrl = command.OriginalUrl,
        Code = code,
        Expires = command.Expires,
      };

    public static CreateEntryUrlDataDto ToDto(this UrlEntry entity) =>
      new(entity.Id.ToString(), entity.Code, entity.OriginalUrl, entity.Expires);
  }
}
