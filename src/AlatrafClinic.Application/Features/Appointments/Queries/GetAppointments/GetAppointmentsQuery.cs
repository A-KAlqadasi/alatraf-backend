using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Enums;
using AlatrafClinic.Domain.Services.Enums;

public sealed record GetAppointmentsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    AppointmentStatus? Status = null,
    PatientType? PatientType = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    string SortColumn = "AttendDate",
    string SortDirection = "asc"
) : ICachedQuery<Result<PaginatedList<AppointmentDto>>>
{
    public string CacheKey =>
        $"appointments:p={Page}:ps={PageSize}" +
        $":q={(SearchTerm ?? "-")}" +
        $":status={(Status?.ToString() ?? "-")}" +
        $":ptype={(PatientType?.ToString() ?? "-")}" +
        $":from={(FromDate?.ToString("yyyyMMdd") ?? "-")}" +
        $":to={(ToDate?.ToString("yyyyMMdd") ?? "-")}" +
        $":sort={SortColumn}:{SortDirection}";

    public string[] Tags => new[] { "appointment" };
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
