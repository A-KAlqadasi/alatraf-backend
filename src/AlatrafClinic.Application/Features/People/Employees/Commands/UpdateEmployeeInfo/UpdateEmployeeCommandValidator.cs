using AlatrafClinic.Application.Features.People.Persons.Services;

using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Employees.Commands.UpdateEmployeeInfo;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
  public UpdateEmployeeCommandValidator()
  {
    RuleFor(x => x.EmployeeId)
        .NotEmpty()
        .WithMessage("EmployeeId is required.");

    RuleFor(x => x.Person)
        .NotNull()
        .SetValidator(new PersonInputValidator()); // ✅ reuse shared validator

    RuleFor(x => x.Role)
        .IsInEnum()
        .WithMessage("Invalid role value.");
  }
}