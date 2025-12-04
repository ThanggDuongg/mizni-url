using FluentValidation;

namespace Application.Features.EntryUrl.DeleteEntryUrl
{
  public class DeleteEntryUrlValidator : AbstractValidator<DeleteEntryUrlCommand>
  {
    public DeleteEntryUrlValidator()
    {
      RuleFor(x => x.Code).NotEmpty();
    }
  }
}
