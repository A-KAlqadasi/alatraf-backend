using FluentValidation;

namespace AlatrafClinic.Application.Features.Organization.Rooms.Commands.CreateRoom;

public sealed class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
{
  public CreateRoomCommandValidator()
  {
    RuleFor(x => x.SectionId)
        .GreaterThan(0)
        .WithMessage("SectionId must be greater than zero.");

    RuleFor(x => x.RoomNumbers)
        .NotNull().WithMessage("At least one room number must be provided.")
        .NotEmpty().WithMessage("At least one room number must be provided.");

    RuleForEach(x => x.RoomNumbers)
        .GreaterThan(0).WithMessage("Room number must be greater than zero.");
  }
}