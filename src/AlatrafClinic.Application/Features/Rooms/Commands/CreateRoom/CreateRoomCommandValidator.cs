using FluentValidation;

namespace AlatrafClinic.Application.Features.Rooms.Commands.CreateRoom;

public sealed class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
{
  public CreateRoomCommandValidator()
  {
    RuleFor(x => x.SectionId)
        .GreaterThan(0)
        .WithMessage("SectionId must be greater than zero.");
    RuleFor(x=> x.Name)
        .NotEmpty()
        .WithMessage("Room name is required");
  }
}