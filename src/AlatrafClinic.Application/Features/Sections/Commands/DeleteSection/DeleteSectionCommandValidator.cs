using FluentValidation;

namespace AlatrafClinic.Application.Features.Sections.Commands.DeleteSection;

public class DeleteSectionCommandValidator : AbstractValidator<DeleteSectionCommand>
{
    public DeleteSectionCommandValidator()
    {
        RuleFor(command => command.SectionId)
            .GreaterThan(0).WithMessage("Section ID must be greater than zero.");
    }
}