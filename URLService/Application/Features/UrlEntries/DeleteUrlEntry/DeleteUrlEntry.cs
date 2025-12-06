using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.UrlEntries.DeleteUrlEntry
{
  public sealed record DeleteUrlEntryCommand(string Code);

  public static class DeleteUrlEntryCommandHandler
  {
    public static async Task Handle(
      DeleteUrlEntryCommand command,
      IUrlContext urlContext,
      CancellationToken cancellationToken
    )
    {
      var code = command.Code.Trim();
      var entity =
        await urlContext
          .Get<UrlEntry>()
          .SingleOrDefaultAsync(e => e.Code == code, cancellationToken)
        ?? throw new UrlEntryNotFoundException(code);

      await urlContext.RemoveAsync(entity);
      await urlContext.SaveChangesAsync(cancellationToken);
    }
  }
}
