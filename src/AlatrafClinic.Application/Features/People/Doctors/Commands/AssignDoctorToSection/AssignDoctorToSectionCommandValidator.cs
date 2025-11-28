using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Doctors.Commands.AssignDoctorToRoom;

public sealed class AssignDoctorToSectionCommandValidator
    : AbstractValidator<AssignDoctorToSectionCommand>
{
  public AssignDoctorToSectionCommandValidator()
  {
    RuleFor(x => x.DoctorId)
        .GreaterThan(0)
        .WithMessage("DoctorId must be greater than zero.");

    RuleFor(x => x.SectionId)
        .GreaterThan(0)
        .WithMessage("Section Id must be greater than zero.");
  }
}