using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Application.Features.Appointments.Mappers;
using AlatrafClinic.Domain.Appointments;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetAppointmentById;

public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, Result<AppointmentDto>>
{
    private readonly ILogger<GetAppointmentByIdQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetAppointmentByIdQueryHandler(ILogger<GetAppointmentByIdQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<AppointmentDto>> Handle(GetAppointmentByIdQuery query, CancellationToken ct)
    {
        Appointment? appointment = await _context.Appointments
        .Include(a=> a.Ticket)
            .ThenInclude(a=> a.Patient!)
                .ThenInclude(p=> p.Person)
        .FirstOrDefaultAsync(a=> a.Id ==query.AppointmentId, ct);

        if (appointment is null)
        {
            _logger.LogWarning("Appointment with ID {AppointmentId} not found.", query.AppointmentId);
            return AppointmentErrors.AppointmentNotFound;
        }

        return appointment.ToDto();
    }
}