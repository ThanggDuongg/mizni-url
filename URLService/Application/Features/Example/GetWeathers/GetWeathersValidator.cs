using FluentValidation;

namespace Application.Features.Example.GetWeathers
{
  public class GetWeathersValidator : AbstractValidator<GetWeathersQuery>
  {
    public GetWeathersValidator()
    {
      RuleFor(x => x.Days).GreaterThan(0).LessThanOrEqualTo(7);
    }
  }
}
