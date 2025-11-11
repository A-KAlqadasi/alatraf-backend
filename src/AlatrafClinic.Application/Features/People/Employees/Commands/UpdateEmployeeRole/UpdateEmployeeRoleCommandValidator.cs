using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Employees.Commands.UpdateEmployeeRole;

public class UpdateEmployeeRoleCommandValidator : AbstractValidator<UpdateEmployeeRoleCommand>
{
  public UpdateEmployeeRoleCommandValidator()
  {
    RuleFor(x => x.EmployeeId)
        .NotEmpty()
        .WithMessage("EmployeeId is required.");

    RuleFor(x => x.Role)
        .IsInEnum()
        .WithMessage("Invalid role value.");
  }
}
