using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Appointments;
using AlatrafClinic.Domain.Services.Appointments.Holidays;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetNextValidAppointmentDate;

public class GetNextValidAppointmentDateQueryHandler(
    IUnitOfWork unitOfWork,
    AppointmentScheduleRules rules,
    HolidayCalendar holidayCalendar) : IRequestHandler<GetNextValidAppointmentDateQuery, Result<DateTime>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly AppointmentScheduleRules _rules = rules;
    private readonly HolidayCalendar _holidayCalendar = holidayCalendar;

    public async Task<Result<DateTime>> Handle(GetNextValidAppointmentDateQuery query, CancellationToken ct)
    {
        DateTime lastScheduleDate = await _unitOfWork.Appointments.GetLastAppointmentDate(ct);

        DateTime baseDate = lastScheduleDate.Date < DateTime.Now.Date ? DateTime.Now.Date : lastScheduleDate.Date;

        if (query.RequestedDate.Date > baseDate)
        {
            baseDate = query.RequestedDate.Date;
        }

        while (!_rules.IsAllowedDay(baseDate.DayOfWeek) || _holidayCalendar.IsHoliday(baseDate))
        {
            baseDate = baseDate.AddDays(1);
        }

        return baseDate;
    }
}