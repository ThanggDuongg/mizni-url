using Application.Services.Interfaces;
using Contracts.UrlEntries.CreateUrlEntry;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Application.Features.UrlEntries.CreateUrlEntry
{
  public sealed record CreateUrlEntryCommand(string OriginalUrl, DateTime? ExpiredAt);

  public static class CreateUrlEntryCommandHandler
  {
    public static async Task<CreateUrlEntryDataDto> Handle(
      CreateUrlEntryCommand command,
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
