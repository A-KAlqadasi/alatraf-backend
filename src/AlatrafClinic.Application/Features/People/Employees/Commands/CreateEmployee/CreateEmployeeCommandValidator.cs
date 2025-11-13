using AlatrafClinic.Application.Features.People.Persons.Services;

using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
  public CreateEmployeeCommandValidator()
  { RuleFor(x => x.Person)
            .NotNull()
            .SetValidator(new PersonInputValidator()); // ✅ reuse existing person validator

   
    RuleFor(x => x.Role)
        .IsInEnum()
        .WithMessage("Invalid role value.");
  }
}
