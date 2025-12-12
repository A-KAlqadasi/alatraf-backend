using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.Appointments.Commands.ScheduleAppointment;

public sealed record class ScheduleAppointmentCommand(int TicketId, PatientType PatientType, DateOnly? RequestedDate = null, string? Notes = null) : IRequest<Result<AppointmentDto>>;