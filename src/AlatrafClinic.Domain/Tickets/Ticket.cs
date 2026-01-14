using AlatrafClinic.Domain.Appointments;
using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Services;

namespace AlatrafClinic.Domain.Tickets;

public class Ticket : AuditableEntity<int>
{
    public int? PatientId { get; private set; }
    public Patient? Patient { get; private set; }
    public int ServiceId { get; private set; }
    public Service Service { get; private set; } = default!;
    public decimal? ServicePrice { get; private set; } = null;
    public TicketStatus Status { get; private set; } = TicketStatus.New;
    public bool IsEditable => Status != TicketStatus.Completed && Status != TicketStatus.Cancelled;

    // navigations
    public Diagnosis? Diagnosis { get; set; }
    public Appointment? Appointment { get; set; }
    public Payment? Payment { get; private set; }

    private Ticket() { }

    private Ticket(Patient? patient, Service service)
    {
        Patient = patient;
        PatientId = patient?.Id;
        Service = service;
        ServicePrice = service.Price;
        ServiceId = service.Id;
    }
    public static Result<Ticket> Create(Patient? patient, Service service)
    {
        if (service is null)
        {
            return TicketErrors.ServiceIsRequired;
        }

        if (service.Id != 1 && patient is null)
        {
            return TicketErrors.PatientIsRequired;
        }

        return new Ticket(patient, service);
    }

    public Result<Updated> Update(Patient patient, Service service, TicketStatus? status = null)
    {
        if (!IsEditable)
        {
            return TicketErrors.ReadOnly;
        }
        
        if (patient is null)
        {
            return TicketErrors.PatientIsRequired;
        }

        if (service is null)
        {
            return TicketErrors.ServiceIsRequired;
        }
        Patient = patient;
        PatientId = patient.Id;
        Service = service;
        ServiceId = service.Id;
        ServicePrice = service.Price;
        Status = status ?? Status;

        return Result.Updated;
    }

    public Result<Updated> AssignDiagnosis(Diagnosis diagnosis)
    {
        if (Diagnosis is not null)
        {
            return TicketErrors.DiagnosisAlreadyAssigned;
        }

        if (!CanTransitionTo(TicketStatus.Completed))
        {
            return TicketErrors.InvalidStateTransition(Status, TicketStatus.Completed);
        }
        if(Id != diagnosis.TicketId)
        {
            return TicketErrors.DiagnosisTicketMismatch;
        }

        Status = TicketStatus.Completed;
        Diagnosis = diagnosis;

        return Result.Updated;
    }

    public Result<Updated> AssignAppointment(Appointment appointment)
    {
        if (Appointment is not null)
        {
            return TicketErrors.AppointmentAlreadyAssigned;
        }

        if(Id != appointment.TicketId)
        {
            return TicketErrors.AppointmentTicketMismatch;
        }

        if (!CanTransitionTo(TicketStatus.Pause))
        {
            return TicketErrors.InvalidStateTransition(Status, TicketStatus.Pause);
        }
        Status = TicketStatus.Pause;
        
        Appointment = appointment;
        return Result.Updated;
    }
    public bool CanTransitionTo(TicketStatus newStatus)
    {
        return (Status, newStatus) switch
        {
            (TicketStatus.New, TicketStatus.Pause) => true,
            (TicketStatus.Pause, TicketStatus.Continue) => true,
            (TicketStatus.New, TicketStatus.Completed) => true,
            (TicketStatus.Continue, TicketStatus.Completed) => true,
            (_, TicketStatus.Cancelled) when Status is not TicketStatus.Completed => true,
            _ => false
        };
    }

    public Result<Updated> Cancel()
    {
        if (!CanTransitionTo(TicketStatus.Cancelled))
        {
            return TicketErrors.InvalidStateTransition(Status, TicketStatus.Cancelled);
        }

        Status = TicketStatus.Cancelled;
        return Result.Updated;
    }
    public Result<Updated> Continue()
    {
        if (!CanTransitionTo(TicketStatus.Continue))
        {
            return TicketErrors.InvalidStateTransition(Status, TicketStatus.Continue);
        }

        Status = TicketStatus.Continue;
        return Result.Updated;
    }
    public Result<Updated> Complete()
    {
        if (!CanTransitionTo(TicketStatus.Completed))
        {
            return TicketErrors.InvalidStateTransition(Status, TicketStatus.Completed);
        }

        Status = TicketStatus.Completed;
        return Result.Updated;
    }
    public Result<Updated> Pause()
    {
        if (!CanTransitionTo(TicketStatus.Pause))
        {
            return TicketErrors.InvalidStateTransition(Status, TicketStatus.Pause);
        }

        Status = TicketStatus.Pause;
        return Result.Updated;
    }
}