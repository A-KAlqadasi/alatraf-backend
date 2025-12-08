using FluentValidation;

namespace AlatrafClinic.Application.Features.Patients.Commands.DeletePatient;

public class DeletePatientCommandValidator : AbstractValidator<DeletePatientCommand>
{
    public DeletePatientCommandValidator()
    {
        RuleFor(x=> x.PatientId)
            .GreaterThan(0)
            .WithMessage("Patient id must be greater than zero");
    }
}