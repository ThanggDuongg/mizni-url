using Contracts.UrlEntries.CreateUrlEntry;
using Domain.Entities;

namespace Application.Features.UrlEntries.CreateUrlEntry
{
  public static class CreateUrlEntryMapping
  {
    public static CreateUrlEntryCommand ToCommand(this CreateUrlEntryRequestDto dto) =>
      new(dto.OriginalUrl, dto.ExpiredAt);

    public static UrlEntry ToEntity(this CreateUrlEntryCommand command, string code) =>
      new()
      {
        OriginalUrl = command.OriginalUrl,
        Code = code,
        ExpiredAt = command.ExpiredAt,
      };

    public static CreateUrlEntryDataDto ToDto(this UrlEntry entity) =>
      new(entity.Id.ToString(), entity.Code, entity.OriginalUrl, entity.ExpiredAt);
  }
}
