using FluentValidation;

namespace AlatrafClinic.Application.Features.Departments.Commands.UpdateDepartment;

public sealed class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
{
  public UpdateDepartmentCommandValidator()
  {
    RuleFor(x => x.DepartmentId)
        .GreaterThan(0)
        .WithMessage("DepartmentId must be greater than zero.");

    RuleFor(x => x.NewName)
        .NotEmpty().WithMessage("New department name is required.")
        .MaximumLength(100).WithMessage("Department name cannot exceed 100 characters.");
  }
}
