using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Employees.Commands.UpdateEmployeeRole;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
  public UpdateEmployeeCommandValidator()
  {
    RuleFor(x => x.EmployeeId)
        .NotEmpty()
        .WithMessage("EmployeeId is required.");

    RuleFor(x => x.Role)
        .IsInEnum()
        .WithMessage("Invalid role value.");
  }
}
