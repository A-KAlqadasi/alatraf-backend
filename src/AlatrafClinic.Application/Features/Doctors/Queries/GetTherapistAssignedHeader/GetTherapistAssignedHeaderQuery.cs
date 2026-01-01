using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTherapistAssignedHeader;

public sealed record GetTherapistAssignedHeaderQuery(
    int DoctorSectionRoomId
) : IRequest<Result<TherapistHeaderDto>>;
