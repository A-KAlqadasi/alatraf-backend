using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Queries.GetPersonById;

public sealed class GetPersonByIdValidator : AbstractValidator<GetPersonByIdQuery>
{
  public GetPersonByIdValidator()
  {
    RuleFor(x => x.PersonId)
        .GreaterThan(0)
        .NotEmpty()
        .WithMessage("PersonId must be greater than zero and not Empty.");
  }
}