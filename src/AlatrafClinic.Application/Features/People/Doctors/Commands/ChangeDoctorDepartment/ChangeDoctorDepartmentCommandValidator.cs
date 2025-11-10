using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Doctors.Commands.ChangeDoctorDepartment;

public sealed class ChangeDoctorDepartmentCommandValidator
    : AbstractValidator<ChangeDoctorDepartmentCommand>
{
  public ChangeDoctorDepartmentCommandValidator()
  {
    RuleFor(x => x.DoctorId)
        .GreaterThan(0)
        .WithMessage("DoctorId must be greater than zero.");

    RuleFor(x => x.NewDepartmentId)
        .GreaterThan(0)
        .WithMessage("NewDepartmentId must be greater than zero.");
  }
}
