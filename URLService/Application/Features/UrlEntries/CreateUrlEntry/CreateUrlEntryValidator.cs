using FluentValidation;

namespace Application.Features.UrlEntries.CreateUrlEntry
{
  public class CreateUrlEntryValidator : AbstractValidator<CreateUrlEntryCommand>
  {
    public CreateUrlEntryValidator()
    {
      RuleFor(x => x.OriginalUrl)
        .NotEmpty()
        .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
        .WithMessage("Invalid URL format");

      RuleFor(x => x.Expires)
        .GreaterThan(DateTime.UtcNow)
        .When(x => x.Expires.HasValue)
        .WithMessage("Expires must be in the future");
    }
  }
}
