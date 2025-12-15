
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Doctors.Commands.AssignDoctorToRoom;

public sealed record AssignDoctorToSectionCommand(
    int DoctorId,
    int SectionId,
    string? Notes
) : IRequest<Result<Updated>>;
