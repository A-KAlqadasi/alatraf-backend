using FluentValidation;

namespace AlatrafClinic.Application.Features.Organization.Sections.Commands.CreateSection;

public sealed class CreateSectionCommandValidator : AbstractValidator<CreateSectionCommand>
{
  public CreateSectionCommandValidator()
  {
    RuleFor(x => x.DepartmentId)
        .GreaterThan(0)
        .WithMessage("DepartmentId must be greater than zero.");

    RuleFor(x => x.SectionNames)
        .NotNull()
        .NotEmpty()
        .WithMessage("At least one section name must be provided.");

    RuleForEach(x => x.SectionNames)
        .NotEmpty().WithMessage("Section name cannot be empty.")
        .MaximumLength(100).WithMessage("Section name cannot exceed 100 characters.");
  }
}