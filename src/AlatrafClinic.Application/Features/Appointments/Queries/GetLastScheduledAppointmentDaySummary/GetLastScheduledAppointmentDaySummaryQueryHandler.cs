using System.Globalization;

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Application.Features.Appointments.Queries.GetNextValidAppointmentDate;
using AlatrafClinic.Application.Features.Appointments.Shared;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Appointments.Holidays;
using AlatrafClinic.Domain.Services.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetLastScheduledAppointmentDaySummary;

public sealed class GetLastScheduledAppointmentDaySummaryQueryHandler
    : IRequestHandler<GetLastScheduledAppointmentDaySummaryQuery, Result<AppointmentDaySummaryDto>>
{
    private readonly IAppDbContext _context;
    private readonly ISender _sender;

    public GetLastScheduledAppointmentDaySummaryQueryHandler(IAppDbContext context, ISender sender)
    {
        _context = context;
        _sender = sender;
    }

    public async Task<Result<AppointmentDaySummaryDto>> Handle(GetLastScheduledAppointmentDaySummaryQuery query, CancellationToken ct)
    {
        // Choose the statuses that represent “real scheduling”
        var baseQuery = _context.Appointments.AsNoTracking()
            .Where(a => a.Status != AppointmentStatus.Cancelled);

        var lastAttendDate = await baseQuery
            .MaxAsync(a => (DateOnly?)a.AttendDate, ct);

        if (lastAttendDate is null)
        {
            var nextValidDate = await _sender.Send(new GetNextValidAppointmentDayQuery());

            return new AppointmentDaySummaryDto(Date: nextValidDate.Value.Date, AppointmentsCount: nextValidDate.Value.AppointmentsCountOnThatDate, DayOfWeek: nextValidDate.Value.DayOfWeek
            );
        }

        var count = await baseQuery
            .CountAsync(a => a.AttendDate == lastAttendDate.Value, ct);

        return new AppointmentDaySummaryDto(Date: lastAttendDate.Value, AppointmentsCount: count, DayOfWeek: UtilityService.GetDayNameArabic(lastAttendDate.Value));
    }

    
}