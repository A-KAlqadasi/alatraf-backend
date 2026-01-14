
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Application.Features.Appointments.Services;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetLastScheduledAppointmentDaySummary;

public sealed class GetLastScheduledAppointmentDaySummaryQueryHandler
    : IRequestHandler<GetLastScheduledAppointmentDaySummaryQuery, Result<AppointmentDaySummaryDto>>
{
    private readonly AppointmentSchedulingService _schedulingService;

    public GetLastScheduledAppointmentDaySummaryQueryHandler(
        AppointmentSchedulingService schedulingService)
    {
        _schedulingService = schedulingService;
    }

    public async Task<Result<AppointmentDaySummaryDto>> Handle(
        GetLastScheduledAppointmentDaySummaryQuery query, 
        CancellationToken ct)
    {
        try
        {
            var summary = await _schedulingService.GetLastAppointmentDaySummaryAsync(ct);
            return summary;
        }
        catch (InvalidOperationException ex)
        {
            return Error.Failure("Appointment.NoAvailableDates", ex.Message);
        }
        catch 
        {
            return Error.Failure("Appointment.SummaryError", "Failed to get appointment day summary");
        }
    }
}