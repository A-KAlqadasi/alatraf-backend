using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetNextValidAppointmentDate;

public class GetNextValidAppointmentDateQueryHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<GetNextValidAppointmentDateQuery, Result<DateTime>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<DateTime>> Handle(GetNextValidAppointmentDateQuery query, CancellationToken ct)
    {
       
       DateTime lastAppointmentDate = await _unitOfWork.Appointments.GetLastAppointmentAttendDate(ct);

        DateTime baseDate = lastAppointmentDate.Date < DateTime.Now.Date ? DateTime.Now.Date : lastAppointmentDate.Date;

        if (query.RequestedDate.HasValue && query.RequestedDate.Value.Date > baseDate)
        {
            baseDate = query.RequestedDate.Value.Date;
        }

        var allowedDaysString = await _unitOfWork.AppSettings.GetAllowedAppointmentDaysAsync(ct);
        
        var allowedDays = allowedDaysString.Split(',').Select(day => Enum.Parse<DayOfWeek>(day.Trim())).ToList();

        var holidays = await _unitOfWork.Holidays.GetAllAsync(ct);


        while (!allowedDays.Contains(baseDate.DayOfWeek) || baseDate.DayOfWeek == DayOfWeek.Friday || holidays.Any(h => h.Matches(baseDate)))
        {
            baseDate = baseDate.AddDays(1);
        }

        return baseDate;
    }
}