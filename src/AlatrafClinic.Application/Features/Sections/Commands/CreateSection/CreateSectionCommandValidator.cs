using FluentValidation;

namespace AlatrafClinic.Application.Features.Sections.Commands.CreateSection;

public sealed class CreateSectionCommandValidator : AbstractValidator<CreateSectionCommand>
{
  public CreateSectionCommandValidator()
  {
    RuleFor(x => x.DepartmentId)
        .GreaterThan(0)
        .WithMessage("DepartmentId must be greater than zero.");

    RuleFor(x => x.Name)
        .NotEmpty()
        .WithMessage("Section name is required");
  }
}