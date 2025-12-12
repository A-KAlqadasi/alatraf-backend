using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetNextValidAppointmentDate;

public class GetNextValidAppointmentDateQueryHandler(
    IAppDbContext _context) : IRequestHandler<GetNextValidAppointmentDateQuery, Result<DateOnly>>
{

    public async Task<Result<DateOnly>> Handle(GetNextValidAppointmentDateQuery query, CancellationToken ct)
    {
       
       var lastAppointment = await _context.Appointments.OrderByDescending(a=> a.AttendDate).FirstOrDefaultAsync(ct);

        DateOnly lastAppointmentDate = lastAppointment?.AttendDate ?? DateOnly.MinValue;

        DateOnly baseDate = lastAppointmentDate < AlatrafClinicConstants.TodayDate ? DateOnly.FromDateTime(DateTime.Now) : lastAppointmentDate;

        if (query.RequestedDate.HasValue && query.RequestedDate.Value > baseDate)
        {
            baseDate = query.RequestedDate.Value;
        }

         var allowedDaysString = await _context.AppSettings
            .Where(a => a.Key == AlatrafClinicConstants.AllowedDaysKey)
            .Select(a => a.Value)
            .FirstOrDefaultAsync(ct);
        
        
        var allowedDays = allowedDaysString?.Split(',').Select(day => Enum.Parse<DayOfWeek>(day.Trim())).ToList() ?? [DayOfWeek.Saturday, DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday];

        var holidays = await _context.Holidays.ToListAsync(ct);


        while (!allowedDays.Contains(baseDate.DayOfWeek) || baseDate.DayOfWeek == DayOfWeek.Friday || holidays.Any(h => h.Matches(baseDate)))
        {
            baseDate = baseDate.AddDays(1);
        }

        return baseDate;
    }
}