
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTechnicianAssignedHeader;

public sealed record GetTechnicianAssignedHeaderQuery(
    int DoctorSectionRoomId
) : IRequest<Result<TechnicianHeaderDto>>;
