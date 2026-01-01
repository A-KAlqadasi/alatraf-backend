using System.Globalization;

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetLastScheduledAppointmentDaySummary;

public sealed class GetLastScheduledAppointmentDaySummaryQueryHandler
    : IRequestHandler<GetLastScheduledAppointmentDaySummaryQuery, Result<AppointmentDaySummaryDto>>
{
    private readonly IAppDbContext _context;

    public GetLastScheduledAppointmentDaySummaryQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AppointmentDaySummaryDto>> Handle(GetLastScheduledAppointmentDaySummaryQuery query, CancellationToken ct)
    {
        // Choose the statuses that represent “real scheduling”
        var baseQuery = _context.Appointments.AsNoTracking()
            .Where(a => a.Status != AppointmentStatus.Cancelled);

        var lastDate = await baseQuery
            .MaxAsync(a => (DateOnly?)a.AttendDate, ct);

        if (lastDate is null)
        {
            // No appointments at all
            return new AppointmentDaySummaryDto(Date: default, AppointmentsCount: 0, DayOfWeek: string.Empty);
        }

        var count = await baseQuery
            .CountAsync(a => a.AttendDate == lastDate.Value, ct);

        return new AppointmentDaySummaryDto(Date: lastDate.Value, AppointmentsCount: count, DayOfWeek: UtilityService.GetDayNameArabic(lastDate.Value));
    }

}