using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Patients.Enums;

using FluentValidation;

namespace AlatrafClinic.Application.Features.Appointments.Commands.ScheduleAppointment;

public class ScheduleAppointmentCommandValidator : AbstractValidator<ScheduleAppointmentCommand>
{
    public ScheduleAppointmentCommandValidator()
    {
        RuleFor(x => x.TicketId)
            .GreaterThan(0).WithMessage("Ticket Id is invalid");
        RuleFor(x => x.RequestedDate)
            .Must(date => date == null || date.Value >= AlatrafClinicConstants.TodayDate)
            .WithMessage("Requested Date cannot be in the past");
        
    }
}