using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Services.Appointments;

namespace AlatrafClinic.Domain.Services.Tickets;

public class Ticket : AuditableEntity<int>
{
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }
    public int ServiceId { get; set; }
    public Service? Service { get; set; }

    //public ICollection<Diagnosis> Diagnosises { get; set; } = new List<Diagnosis>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
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
}