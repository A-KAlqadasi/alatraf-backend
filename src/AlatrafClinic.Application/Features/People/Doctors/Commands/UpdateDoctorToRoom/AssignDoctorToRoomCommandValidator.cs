using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Doctors.Commands.AssignDoctorToRoom;

public sealed class AssignDoctorToRoomCommandValidator
    : AbstractValidator<AssignDoctorToRoomCommand>
{
  public AssignDoctorToRoomCommandValidator()
  {
    RuleFor(x => x.DoctorId)
        .GreaterThan(0)
        .WithMessage("DoctorId must be greater than zero.");

    RuleFor(x => x.RoomId)
        .GreaterThan(0)
        .WithMessage("RoomId must be greater than zero.");
  }
}