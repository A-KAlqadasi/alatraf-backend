using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetNextValidAppointmentDate;

public class GetNextValidAppointmentDateQueryHandler(
    IAppDbContext _context) : IRequestHandler<GetNextValidAppointmentDateQuery, Result<DateTime>>
{

    public async Task<Result<DateTime>> Handle(GetNextValidAppointmentDateQuery query, CancellationToken ct)
    {
       
       var lastAppointment = await _context.Appointments.OrderByDescending(a=> a.AttendDate).FirstOrDefaultAsync(ct);

        DateTime lastAppointmentDate = lastAppointment?.AttendDate ?? DateTime.MinValue;

        DateTime baseDate = lastAppointmentDate.Date < DateTime.Now.Date ? DateTime.Now.Date : lastAppointmentDate.Date;

        if (query.RequestedDate.HasValue && query.RequestedDate.Value.Date > baseDate)
        {
            baseDate = query.RequestedDate.Value.Date;
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