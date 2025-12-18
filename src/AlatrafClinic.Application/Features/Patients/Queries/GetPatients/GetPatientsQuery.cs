using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Patients.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.Patients.Queries.GetPatients;

public sealed record GetPatientsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    PatientType? PatientType = null,
    bool? Gender = null,
    DateOnly? BirthdateFrom = null,
    DateOnly? BirthdateTo = null,
    bool? HasNationalNo = null,
    string SortColumn = "fullname",
    string SortDirection = "asc"
) : IRequest<Result<PaginatedList<PatientDto>>>;