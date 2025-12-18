
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;


namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTechniciansDropdown;

public sealed record GetTechniciansDropdownQuery(
    int Page,
    int PageSize,
    int? SectionId = null,
    string? SearchTerm = null
) : IRequest<Result<PaginatedList<TechnicianDto>>>;