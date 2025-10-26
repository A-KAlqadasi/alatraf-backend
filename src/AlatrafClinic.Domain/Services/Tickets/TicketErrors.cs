using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Services.Tickets;

public static class TicketErrors
{
    public static readonly Error PatientIdIsRequired = Error.Validation("Ticket.PatientIdIsRequired", "Patient Id is required");

    public static readonly Error ServiceIdIsRequired = Error.Validation("Ticket.ServiceIdIsRequired", "Service Id is required");

    public static readonly Error DiagnosisAlreadyAssigned = Error.Validation("Ticket.DiagnosisAlreadyAssigned", "Diagnosis is already assigned to this ticket");
    public static readonly Error AppointmentAlreadyAssigned = Error.Validation("Ticket.AppointmentAlreadyAssigned", "Appointment is already assigned to this ticket");
}