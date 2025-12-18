using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Enums;
using AlatrafClinic.Domain.Services.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetAppointments;

public sealed record GetAppointmentsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    AppointmentStatus? Status = null,
    PatientType? PatientType = null,
    DateOnly? FromDate = null,
    DateOnly? ToDate = null,
    string SortColumn = "AttendDate",
    string SortDirection = "asc"
) : IRequest<Result<PaginatedList<AppointmentDto>>>;