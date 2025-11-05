using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Enums;

namespace AlatrafClinic.Domain.Services.Tickets;

public static class TicketErrors
{
    public static readonly Error PatientIdIsRequired = Error.Validation("Ticket.PatientIdIsRequired", "Patient Id is required");

    public static readonly Error ServiceIdIsRequired = Error.Validation("Ticket.ServiceIdIsRequired", "Service Id is required");

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
}