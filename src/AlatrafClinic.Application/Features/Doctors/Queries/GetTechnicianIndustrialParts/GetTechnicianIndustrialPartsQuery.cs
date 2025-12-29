using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTechnicianIndustrialParts;

public sealed record GetTechnicianIndustrialPartsQuery(
    int DoctorSectionRoomId,
    DateOnly? date = null,
    int? repairCardId = null,
    string? patientName = null,
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PaginatedList<TechnicianIndustrialPartDto>>>;
