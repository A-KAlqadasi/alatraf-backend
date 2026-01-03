using FluentValidation;

namespace AlatrafClinic.Application.Features.Sections.Commands.AssignNewRoomsToSection;

public class AssignNewRoomsToSectionCommandValidator : AbstractValidator<AssignNewRoomsToSectionCommand>
{
    public AssignNewRoomsToSectionCommandValidator()
    {
        RuleFor(x => x.SectionId)
            .GreaterThan(0).WithMessage("SectionId must be greater than zero.");

        RuleFor(x => x.RoomNames)
            .NotEmpty().WithMessage("RoomNames list cannot be empty.")
            .Must(roomNames => roomNames.All(name => !string.IsNullOrWhiteSpace(name)))
            .WithMessage("Room names cannot be null or whitespace.");
    }
}