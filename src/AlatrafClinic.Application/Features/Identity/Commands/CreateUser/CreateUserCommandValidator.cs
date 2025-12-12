using AlatrafClinic.Domain.Common.Constants;

using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Fullname)
        .NotEmpty().WithMessage("Fullname is required.")
        .MaximumLength(150);

    RuleFor(x => x.Birthdate)
        .LessThanOrEqualTo(AlatrafClinicConstants.TodayDate).WithMessage("Birthdate cannot be in the future.");

    RuleFor(x => x.Phone)
        .NotEmpty()
        .Matches(@"^(77|78|73|71)\d{7}$")
        .WithMessage("Phone number must start with 77, 78, 73, or 71 and be 9 digits long.");

    RuleFor(x => x.Address)
        .NotEmpty()
        .MaximumLength(250);
    RuleFor(x => x.Gender)
       .NotNull()
       .WithMessage("Gender is required (true = Male, false = Female).");

    RuleFor(x => x.NationalNo)
              .Matches(@"^\d+$")
              .WithMessage("National number must contain only digits.");

    RuleFor(x=> x.UserName)
        .NotEmpty()
        .MinimumLength(4)
        .MaximumLength(12)
        .WithMessage("Username must be between 4 to 12 characters long.");

    RuleFor(x=> x.Password)
        .NotEmpty()
        .MinimumLength(6)
        .MaximumLength(20)
        .Matches(@"^[a-zA-Z0-9]+$")
        .WithMessage("Password must be alphanumeric and between 6 to 20 characters long.");
    }
}