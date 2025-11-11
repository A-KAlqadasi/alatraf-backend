using FluentValidation;

namespace AlatrafClinic.Application.Features.Organization.Rooms.Commands.UpdateRoom;

public sealed class UpdateRoomCommandValidator : AbstractValidator<UpdateRoomCommand>
{
  public UpdateRoomCommandValidator()
  {
    RuleFor(x => x.RoomId)
        .GreaterThan(0)
        .WithMessage("RoomId must be greater than zero.");

    RuleFor(x => x.NewNumber)
        .GreaterThan(0)
        .WithMessage("Room number must be greater than zero.");
  }
}