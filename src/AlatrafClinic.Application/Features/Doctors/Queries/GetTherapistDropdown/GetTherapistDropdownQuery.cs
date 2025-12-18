
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTherapistDropdown;

public sealed record GetTherapistDropdownQuery(
    int Page,
    int PageSize,
    int? SectionId = null,
    int? RoomId = null,
    string? SearchTerm = null
) : IRequest<Result<PaginatedList<TherapistDto>>>;