using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Application.Features.Appointments.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Appointments;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetAppointmentById;

public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, Result<AppointmentDto>>
{
    private readonly ILogger<GetAppointmentByIdQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetAppointmentByIdQueryHandler(ILogger<GetAppointmentByIdQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<AppointmentDto>> Handle(GetAppointmentByIdQuery query, CancellationToken ct)
    {
        Appointment? appointment = await _unitOfWork.Appointments.GetByIdAsync(query.AppointmentId, ct);
        if (appointment is null)
        {
            _logger.LogWarning("Appointment with ID {AppointmentId} not found.", query.AppointmentId);
            return AppointmentErrors.AppointmentNotFound;
        }

        return appointment.ToDto();
    }
}