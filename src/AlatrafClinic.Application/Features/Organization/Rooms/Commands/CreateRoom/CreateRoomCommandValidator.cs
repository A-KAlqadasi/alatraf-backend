using FluentValidation;

namespace AlatrafClinic.Application.Features.Organization.Rooms.Commands.CreateRoom;

public sealed class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
{
  public CreateRoomCommandValidator()
  {
    RuleFor(x => x.SectionId)
        .GreaterThan(0)
        .WithMessage("SectionId must be greater than zero.");

    RuleFor(x => x.RoomNames)
        .NotNull().WithMessage("At least one room name must be provided.")
        .NotEmpty().WithMessage("At least one room name must be provided.");

    RuleForEach(x => x.RoomNames)
        .NotEmpty().WithMessage("Room name must be provided.");
  }
}