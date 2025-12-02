using FluentValidation;

namespace AlatrafClinic.Application.Features.Doctors.Commands.CreateDoctor;

public sealed class CreateDoctorCommandValidator : AbstractValidator<CreateDoctorCommand>
{
    public CreateDoctorCommandValidator()
    {
        RuleFor(x => x.Fullname)
            .NotEmpty().WithMessage("Fullname is required.")
            .MaximumLength(150);

        RuleFor(x => x.Birthdate)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Birthdate cannot be in the future.");

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

        RuleFor(x => x.NationalNo!)
                .Matches(@"^\d+$")
                .WithMessage("National number must contain only digits.");

        RuleFor(x => x.Specialization)
            .NotEmpty().WithMessage("Specialization is required.")
            .MaximumLength(100).WithMessage("Specialization cannot exceed 100 characters.");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("DepartmentId must be greater than zero.");
    }
}