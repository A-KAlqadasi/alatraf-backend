using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Persons.Commands.UpdatePerson;

public sealed class UpdatePersonCommandValidator : AbstractValidator<UpdatePersonCommand>
{
  public UpdatePersonCommandValidator()
  {
    RuleFor(x => x.PersonId)
        .GreaterThan(0)
        .NotEmpty()
        .WithMessage("PersonId must be  greater than zero and not empety.");

    RuleFor(x => x.Fullname)
        .NotEmpty().WithMessage("Fullname is required.")
        .MaximumLength(150).WithMessage("Fullname cannot exceed 150 characters.");

    RuleFor(x => x.Birthdate)
        .NotNull().WithMessage("Birthdate is required.")
        .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Birthdate cannot be in the future.");

    RuleFor(x => x.Phone)
        .NotEmpty().WithMessage("Phone number is required.")
        .Matches(@"^(77|78|73|71)\d{7}$")
        .WithMessage("Phone number must start with 77, 78, 73, or 71 and be 9 digits long.");
   RuleFor(x => x.Gender)
       .NotNull()
       .WithMessage("Gender is required (true = Male, false = Female).");

    RuleFor(x => x.Address)
        .NotEmpty().WithMessage("Address is required.")
        .MaximumLength(250).WithMessage("Address cannot exceed 250 characters.");

    When(x => !string.IsNullOrWhiteSpace(x.NationalNo), () =>
    {
      RuleFor(x => x.NationalNo!)
              .Matches(@"^\d+$")
              .WithMessage("National number must contain only digits.");
    });
  }
}