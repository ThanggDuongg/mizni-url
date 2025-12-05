using FluentValidation;

namespace Application.Features.UrlEntries.DeleteUrlEntry
{
  public class DeleteUrlEntryValidator : AbstractValidator<DeleteUrlEntryCommand>
  {
    public DeleteUrlEntryValidator()
    {
      RuleFor(x => x.Code).NotEmpty();
    }
  }
}
