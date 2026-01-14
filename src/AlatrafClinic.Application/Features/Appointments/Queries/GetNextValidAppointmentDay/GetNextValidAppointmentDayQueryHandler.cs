using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Application.Features.Appointments.Services;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetNextValidAppointmentDate;

public sealed class GetNextValidAppointmentDayQueryHandler
    : IRequestHandler<GetNextValidAppointmentDayQuery, Result<NextAppointmentDayDto>>
{
    private readonly AppointmentSchedulingService _schedulingService;
    
    public GetNextValidAppointmentDayQueryHandler(AppointmentSchedulingService schedulingService)
    {
        _schedulingService = schedulingService;
    }
    
    public async Task<Result<NextAppointmentDayDto>> Handle(
        GetNextValidAppointmentDayQuery query, 
        CancellationToken ct)
    {
        try
        {
            var nextDate = await _schedulingService.GetNextValidDateForDisplayAsync(
                query.AfterDate, 
                ct);
            
            var appointmentCount = await _schedulingService.GetAppointmentCountForDateAsync(
                nextDate, 
                null, 
                ct);
            
            return new NextAppointmentDayDto(
                nextDate, 
                UtilityService.GetDayNameArabic(nextDate), 
                appointmentCount);
        }
        catch (InvalidOperationException ex)
        {
            // Handle no available dates
            return Error.Failure("Appointment.NoAvailableDates", ex.Message);
        }
    }
}
