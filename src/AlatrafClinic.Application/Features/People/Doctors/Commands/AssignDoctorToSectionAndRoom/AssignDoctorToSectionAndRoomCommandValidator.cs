using FluentValidation;

namespace AlatrafClinic.Application.Features.People.Doctors.Commands.AssignDoctorToSectionAndRoom;

public sealed class AssignDoctorToSectionAndRoomCommandValidator
    : AbstractValidator<AssignDoctorToSectionAndRoomCommand>
{
    public AssignDoctorToSectionAndRoomCommandValidator()
    {
        RuleFor(x => x.DoctorId)
            .GreaterThan(0)
            .WithMessage("DoctorId must be greater than zero.");

        RuleFor(x => x.SectionId)
            .GreaterThan(0)
            .WithMessage("SectionId must be greater than zero.");

        RuleFor(x => x.RoomId)
            .GreaterThan(0)
            .WithMessage("RoomId must be greater than zero.");
    }
}
