using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Enums;

namespace AlatrafClinic.Domain.Services.Tickets;

public static class TicketErrors
{
    public static readonly Error PatientIsRequired = Error.Validation("Ticket.PatientIsRequired", "Patient is required");

    public static readonly Error ServiceIsRequired = Error.Validation("Ticket.ServiceIsRequired", "Service is required");

    public static readonly Error DiagnosisAlreadyAssigned = Error.Validation("Ticket.DiagnosisAlreadyAssigned", "Diagnosis is already assigned to this ticket");
    public static readonly Error AppointmentAlreadyAssigned = Error.Validation("Ticket.AppointmentAlreadyAssigned", "Appointment is already assigned to this ticket");
    public static Error InvalidStateTransition(TicketStatus current, TicketStatus next) => Error.Conflict(
       code: "Ticket.InvalidStateTransition",
       description: $"Ticket Invalid State transition from '{current}' to '{next}'.");
    public static Error ReadOnly = Error.Conflict(
       code: "Ticket.ReadOnly",
       description: "Ticket is not editable");
    public static readonly Error DiagnosisTicketMismatch = Error.Validation("Ticket.DiagnosisTicketMismatch", "The diagnosis does not belong to this ticket");
    public static readonly Error AppointmentTicketMismatch = Error.Validation("Ticket.AppointmentTicketMismatch", "The appointment does not belong to this ticket");
    public static readonly Error TicketNotFound = Error.NotFound("Ticket.NotFound", "Ticket not found");
    public static readonly Error TicketAlreadHasAppointment = Error.Conflict("Ticket.AlreadHasAppointment", "Ticket already has appointment");
    public static readonly Error TicketPaused = Error.Conflict("Ticket.Paused", "Ticket is paused and cannot accept this operation");

    public static readonly Error TicketServiceIsNotRenewal = Error.Conflict("Ticket.TicketServiceIsNotRenewal", "Ticket service is not renew");
    
}