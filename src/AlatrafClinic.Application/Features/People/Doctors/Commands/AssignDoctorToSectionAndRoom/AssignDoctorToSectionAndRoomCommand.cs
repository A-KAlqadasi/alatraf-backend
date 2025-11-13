
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Doctors.Commands.AssignDoctorToSectionAndRoom;

public sealed record AssignDoctorToSectionAndRoomCommand(
    int DoctorId,
    int SectionId,
    int RoomId,
    string? Notes
) : IRequest<Result<Updated>>;
