using FluentValidation;

namespace AlatrafClinic.Application.Features.Doctors.Commands.EndDoctorAssignment;

public sealed class EndDoctorAssignmentCommandValidator
    : AbstractValidator<EndDoctorAssignmentCommand>
{
  public EndDoctorAssignmentCommandValidator()
  {
    RuleFor(x => x.DoctorId)
        .GreaterThan(0)
        .WithMessage("DoctorId must be greater than zero.");
  }
}
