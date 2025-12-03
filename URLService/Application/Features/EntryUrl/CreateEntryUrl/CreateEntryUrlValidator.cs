using FluentValidation;

namespace Application.Features.EntryUrl.CreateEntryUrl
{
  public class CreateEntryUrlValidator : AbstractValidator<CreateEntryUrlCommand>
  {
    public CreateEntryUrlValidator()
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
