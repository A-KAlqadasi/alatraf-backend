using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Services.Appointments;

namespace AlatrafClinic.Domain.Services.Tickets;

public class Ticket : AuditableEntity<int>
{
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }
    public int ServiceId { get; set; }
    public Service? Service { get; set; }

    public Diagnosis? Diagnosis { get; set; }
    public Appointment? Appointment { get; set; }
    private Ticket() { }

    private Ticket(int patientId, int serviceId)
    {
        PatientId = patientId;
        ServiceId = serviceId;
    }
    public static Result<Ticket> Create(int patientId, int serviceId)
    {
        if (patientId <= 0)
        {
            return TicketErrors.PatientIdIsRequired;
        }

        if (serviceId <= 0)
        {
            return TicketErrors.ServiceIdIsRequired;
        }

        return new Ticket(patientId, serviceId);
    }
    public Result<Updated> Update(int patientId, int serviceId)
    {
        if (patientId <= 0)
        {
            return TicketErrors.PatientIdIsRequired;
        }

        if (serviceId <= 0)
        {
            return TicketErrors.ServiceIdIsRequired;
        }
        PatientId = patientId;
        ServiceId = serviceId;

        return Result.Updated;
    }
    public Result<Updated> AssignDiagnosis(Diagnosis diagnosis)
    {
        if (Diagnosis is not null)
        {
            return TicketErrors.DiagnosisAlreadyAssigned;
        }
        Diagnosis = diagnosis;

        return Result.Updated;
    }
    
    public Result<Updated> AssignAppointment(Appointment appointment)
    {
        if (Appointment is not null)
        {
            return TicketErrors.AppointmentAlreadyAssigned;
        }
        Appointment = appointment;

        return Result.Updated;
    }
}