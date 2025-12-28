using FluentValidation;

namespace Application.Features.UrlEntries.UpdateUrlEntry
{
  public class UpdateUrlEntryValidator : AbstractValidator<UpdateUrlEntryCommand>
  {
    public UpdateUrlEntryValidator()
    {
      RuleFor(x => x.Code).NotEmpty();

      RuleFor(x => x.ExpiredAt)
        .Must(x => x == null || x > DateTime.UtcNow)
        .WithMessage("Expires must be greater than now or null.");

      RuleFor(x => x.Version).GreaterThan(0);
    }
  }
}
