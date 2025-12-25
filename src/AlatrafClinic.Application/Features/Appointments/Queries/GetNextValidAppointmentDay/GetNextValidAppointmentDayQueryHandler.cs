using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Application.Features.Appointments.Shared;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetNextValidAppointmentDate;

public sealed class GetNextValidAppointmentDayQueryHandler
    : IRequestHandler<GetNextValidAppointmentDayQuery, Result<NextAppointmentDayDto>>
{
    private readonly IAppDbContext _context;

    public GetNextValidAppointmentDayQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<NextAppointmentDayDto>> Handle(GetNextValidAppointmentDayQuery query, CancellationToken ct)
    {
        // 1) Load rules
        var allowedDaysString = await _context.AppSettings.AsNoTracking()
            .Where(a => a.Key == AlatrafClinicConstants.AllowedDaysKey)
            .Select(a => a.Value)
            .FirstOrDefaultAsync(ct);

        var allowedDays = AppointmentSchedulingCalculator.ParseAllowedDaysOrDefault(allowedDaysString);

        var holidays = await _context.Holidays.AsNoTracking().ToListAsync(ct);

        // 2) Determine the “after” date
        DateOnly afterDate;

        if (query.AfterDate.HasValue)
        {
            afterDate = query.AfterDate.Value;
        }
        else
        {
            // fallback: after last scheduled day, else after today
            var lastDate = await _context.Appointments.AsNoTracking()
                .Where(a => a.Status != AppointmentStatus.Cancelled)
                .MaxAsync(a => (DateOnly?)a.AttendDate, ct);

            afterDate = lastDate ?? AlatrafClinicConstants.TodayDate;
        }

        // 3) Compute next valid day (exclusive)
        var nextValid = AppointmentSchedulingCalculator.GetNextValidDateExclusive(afterDate, allowedDays, holidays);

        // 4) Count appointments on that day (same status policy as you choose)
        var count = await _context.Appointments.AsNoTracking()
            .Where(a => a.Status != AppointmentStatus.Cancelled && a.AttendDate == nextValid)
            .CountAsync(ct);

        return new NextAppointmentDayDto(nextValid, count);
    }
}