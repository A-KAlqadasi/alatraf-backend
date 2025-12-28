using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Patients.Mappers;
using AlatrafClinic.Application.Features.Tickets.Dtos;
using AlatrafClinic.Application.Features.Tickets.Mappers;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Tickets;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Tickets.Queries.GetTicketForScheduleAppointment;

public class GetTicketForScheduleAppointmentQueryHandler : IRequestHandler<GetTicketForScheduleAppointmentQuery, Result<TicketForServiceDto>>
{
    private readonly ILogger<GetTicketForScheduleAppointmentQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetTicketForScheduleAppointmentQueryHandler(ILogger<GetTicketForScheduleAppointmentQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<TicketForServiceDto>> Handle(GetTicketForScheduleAppointmentQuery query, CancellationToken ct)
    {
        var ticket = await _context.Tickets
            .Where(t => t.Id == query.TicketId)
            .Select(t => new TicketForServiceDto
            {
                TicketId = t.Id,
                TicketStatus = t.Status.ToArabicTicketStatus(),
                PatientId = t.PatientId ?? 0,
                ServiceId = t.ServiceId,
                Age = UtilityService.CalculateAge(t.Patient!.Person.Birthdate, AlatrafClinicConstants.TodayDate),
                Gender = UtilityService.GenderToArabicString(t.Patient.Person.Gender),
                ServiceName = t.Service!.Name,
                PatientName = t.Patient!.Person.FullName,
                PhoneNumber = t.Patient!.Person.Phone,
                PatientType = t.Patient!.PatientType.ToArabicPatientType(),
            })
            .FirstOrDefaultAsync();
        if (ticket is null)
        {
            _logger.LogWarning("Ticket with ID {TicketId} not found.", query.TicketId);
            return TicketErrors.TicketNotFound;;
        }
        return ticket;
    }
}