using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.People.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Doctors.Queries.GetDoctorsWithCurrentAssignment;

public sealed record GetDoctorsWithCurrentAssignmentQuery(
    int Page = 1,
    int PageSize = 20,
    int? DepartmentId = null,
    int? SectionId = null,
    int? RoomId = null,
    string? Search = null,
    string? Specialization = null,
    bool? HasActiveAssignment = null,
    string SortBy = "assigndate",
    string SortDir = "desc"
) : IRequest<Result<PaginatedList<DoctorListItemDto>>>;