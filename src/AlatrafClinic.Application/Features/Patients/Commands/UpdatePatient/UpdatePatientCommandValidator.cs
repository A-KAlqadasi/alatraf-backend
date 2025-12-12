using AlatrafClinic.Domain.Common.Constants;

using FluentValidation;

namespace AlatrafClinic.Application.Features.Patients.Commands.UpdatePatient;

public class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
{
    public UpdatePatientCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .GreaterThan(0)
            .WithMessage("PatientId must be greater than zero.");

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

        When(x => !string.IsNullOrWhiteSpace(x.NationalNo), () =>
        {
        RuleFor(x => x.NationalNo!)
                .Matches(@"^\d+$")
                .WithMessage("National number must contain only digits.");
        });

        RuleFor(x => x.PatientType)
            .IsInEnum()
            .WithMessage("Invalid PatientType.");
    }
}
