using FluentValidation;

namespace AlatrafClinic.Application.Features.Organization.Departments.Commands;

public sealed class RenameDepartmentCommandValidator : AbstractValidator<RenameDepartmentCommand>
{
  public RenameDepartmentCommandValidator()
  {
    RuleFor(x => x.DepartmentId)
        .GreaterThan(0)
        .WithMessage("DepartmentId must be greater than zero.");

    RuleFor(x => x.NewName)
        .NotEmpty().WithMessage("New department name is required.")
        .MaximumLength(100).WithMessage("Department name cannot exceed 100 characters.");
  }
}
