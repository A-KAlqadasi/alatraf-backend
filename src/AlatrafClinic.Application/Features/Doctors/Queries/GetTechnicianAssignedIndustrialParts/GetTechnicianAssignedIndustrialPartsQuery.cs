using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTechnicianAssignedIndustrialParts;

public sealed record GetTechnicianAssignedIndustrialPartsQuery(int DoctorSectionRoomId) : IRequest<Result<TechnicianIndustrialPartsResultDto>>;
