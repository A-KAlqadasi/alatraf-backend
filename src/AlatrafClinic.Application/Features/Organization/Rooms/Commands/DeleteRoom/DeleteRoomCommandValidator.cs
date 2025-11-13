using FluentValidation;

namespace AlatrafClinic.Application.Features.Organization.Rooms.Commands.DeleteRoom;

public sealed class DeleteRoomCommandValidator : AbstractValidator<DeleteRoomCommand>
{
  public DeleteRoomCommandValidator()
  {
    RuleFor(x => x.RoomId)
        .GreaterThan(0)
        .WithMessage("RoomId must be greater than zero.");
  }
}
