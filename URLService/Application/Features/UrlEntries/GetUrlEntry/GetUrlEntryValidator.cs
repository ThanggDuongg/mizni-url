using FluentValidation;

namespace Application.Features.UrlEntries.GetUrlEntry
{
  public class GetUrlEntryValidator : AbstractValidator<GetUrlEntryQuery>
  {
    public GetUrlEntryValidator()
    {
      RuleFor(x => x.Code).NotEmpty();
    }
  }
}
