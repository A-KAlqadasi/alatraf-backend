using AlatrafClinic.Application.Features.People.Persons.Services;

using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Patients.Commands.CreatePatient;

public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientCommandValidator()
    {
       RuleFor(x => x.Person)
            .NotNull()
            .SetValidator(new PersonInputValidator()); // 👈 reuse

        RuleFor(x => x.PatientType)
            .IsInEnum()
            .WithMessage("Invalid PatientType.");

        // RuleFor(x => x.AutoRegistrationNumber)
        //     .MaximumLength(50)
        //     .When(x => !string.IsNullOrWhiteSpace(x.AutoRegistrationNumber))
        //     .WithMessage("AutoRegistrationNumber cannot exceed 50 characters.");
    }
}