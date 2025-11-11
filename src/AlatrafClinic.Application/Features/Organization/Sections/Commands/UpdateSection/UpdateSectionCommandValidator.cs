
using FluentValidation;

namespace AlatrafClinic.Application.Features.Organization.Sections.Commands.UpdateSection;

public sealed class UpdateSectionCommandValidator : AbstractValidator<UpdateSectionCommand>
{
  public UpdateSectionCommandValidator()
  {
    RuleFor(x => x.SectionId)
        .GreaterThan(0)
        .WithMessage("SectionId must be greater than zero.");

    RuleFor(x => x.NewName)
        .NotEmpty().WithMessage("New section name is required.")
        .MaximumLength(100).WithMessage("Section name cannot exceed 100 characters.");
  }
}
