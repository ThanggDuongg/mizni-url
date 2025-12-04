using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.EntryUrl.DeleteEntryUrl
{
  public sealed record DeleteEntryUrlCommand(string Code);

  public static class DeleteEntryUrlCommandHandler
  {
    public static async Task Handle(
      DeleteEntryUrlCommand command,
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
