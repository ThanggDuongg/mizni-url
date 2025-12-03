using Application.Services.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Application.Features.EntryUrl.CreateEntryUrl
{
  public sealed record CreateEntryUrlCommand(string OriginalUrl, DateTime? Expires);

  public static class CreateEntryUrlCommandHandler
  {
    public static async Task<CreateEntryUrlDataDto> Handle(
      CreateEntryUrlCommand command,
      ISequenceService sequenceService,
      ICodeGenerator codeGenerator,
      IUrlContext urlContext,
      CancellationToken cancellationToken
    )
    {
      var incr = await sequenceService.GetNext(nameof(UrlEntry), cancellationToken);
      var code = codeGenerator.Encode(incr);

      var entity = command.ToEntity(code);

      await urlContext.AddAsync(entity, cancellationToken);
      await urlContext.SaveChangesAsync(cancellationToken);

      return entity.ToDto();
    }
  }
}
