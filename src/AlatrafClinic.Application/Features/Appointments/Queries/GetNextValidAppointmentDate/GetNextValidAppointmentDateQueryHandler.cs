using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Appointments;
using AlatrafClinic.Domain.Services.Appointments.Holidays;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetNextValidAppointmentDate;

public class GetNextValidAppointmentDateQueryHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<GetNextValidAppointmentDateQuery, Result<DateTime>>
{
  private readonly IUnitOfWork _uow = unitOfWork;

  public async Task<Result<DateTime>> Handle(GetNextValidAppointmentDateQuery request, CancellationToken ct)
  {
    var holidays = await _uow.Holidays.GetHolidaysQueryAsync(ct);
    var holidayList = await holidays.ToListAsync(ct);
    var holidayCalendar = new HolidayCalendar(holidayList);

    // 2️⃣ Get allowed appointment days (example: Mon-Thu)
    // You can also fetch doctor/patient-specific rules if needed
    var allowedDays = new AppointmentScheduleRules(new[]
    {
            DayOfWeek.Saturday,
            DayOfWeek.Sunday,
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday
        });

    // 3️⃣ Start from today (or tomorrow if needed)
    var baseDate = DateTime.UtcNow.Date;

    // 4️⃣ Find next allowed date
    while (!allowedDays.IsAllowedDay(baseDate.DayOfWeek) || holidayCalendar.IsHoliday(baseDate))
    {
      baseDate = baseDate.AddDays(1);
    }

    return baseDate;
  }
}