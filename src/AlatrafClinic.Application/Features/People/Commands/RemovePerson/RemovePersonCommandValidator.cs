using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Commands.RemovePerson;

public sealed class RemovePersonCommandValidator : AbstractValidator<RemovePersonCommand>
{
  public RemovePersonCommandValidator()
  {
    RuleFor(x => x.PersonId)
         .GreaterThan(0)
         .NotEmpty()
         .WithMessage("PersonId must be  greater than zero and not empety.");
  }
}