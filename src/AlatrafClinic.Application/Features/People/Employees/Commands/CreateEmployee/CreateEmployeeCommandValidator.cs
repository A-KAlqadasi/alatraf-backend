using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
  public CreateEmployeeCommandValidator()
  {
    RuleFor(x => x.EmployeeId)
        .NotEmpty()
        .WithMessage("EmployeeId is required.");

    RuleFor(x => x.PersonId)
        .GreaterThan(0)
        .NotEmpty()
        .WithMessage("PersonId is required and  must be greater than zero.");

    RuleFor(x => x.Role)
        .IsInEnum()
        .WithMessage("Invalid role value.");
  }
}
