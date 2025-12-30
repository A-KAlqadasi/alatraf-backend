using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Commands.AssignDoctorToSectionAndRoom;

public sealed record AssignDoctorToSectionAndRoomCommand(
    int DoctorId,
    int SectionId,
    bool IsActive,
    int? RoomId,
    string? Notes
) : IRequest<Result<Updated>>;
