using FluentValidation;

namespace AlatrafClinic.Application.Features.Organization.Departments.Commands.CreateDepartment;

public sealed class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
  public CreateDepartmentCommandValidator()
  {
    RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Department name is required.")
        .MaximumLength(100).WithMessage("Department name cannot exceed 100 characters.");
  }
}