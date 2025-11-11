using AlatrafClinic.Application.Features.People.Persons.Services;

using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Patients.Commands.UpdatePatient;

public class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
{
    public UpdatePatientCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .GreaterThan(0)
            .WithMessage("PatientId must be greater than zero.");

        RuleFor(x => x.Person)
            .NotNull()
            .SetValidator(new PersonInputValidator()); 

        RuleFor(x => x.PatientType)
            .IsInEnum()
            .WithMessage("Invalid PatientType.");
    }
}
