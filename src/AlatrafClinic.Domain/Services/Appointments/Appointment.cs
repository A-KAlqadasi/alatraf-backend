using System.Data;

using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Enums;
using AlatrafClinic.Domain.Services.Enums;
using AlatrafClinic.Domain.Services.Appointments.Holidays;
using AlatrafClinic.Domain.Services.Tickets;

namespace AlatrafClinic.Domain.Services.Appointments;

public class Appointment : AuditableEntity<int>
{
    public PatientType PatientType { get; private set; }
    public DateTime AttendDate { get; private set; }
    public AppointmentStatus Status { get; private set; } 
    public string? Notes { get; private set; }
    public int TicketId { get; private set; }
    public Ticket? Ticket { get; set; }
    public static IReadOnlyCollection<DayOfWeek>? AllowedDays { get; private set; }


    private Appointment() { }

    private Appointment(
        int ticketId,
        PatientType patientType,
        DateTime attendDate,
        AppointmentStatus state,
        string? notes)
    {
        TicketId = ticketId;
        PatientType = patientType;
        AttendDate = attendDate;
        Status = state;
        Notes = notes;
    }

    public static Result<Appointment> Schedule(
        int ticketId,
        PatientType patientType,
        DateTime? requestedDate,
        string? notes,
        DateTime? lastScheduledDate,
        AppointmentScheduleRules rules,
        HolidayCalendar holidays)
    {
        if (ticketId <= 0)
        {
            return AppointmentErrors.TicketIdRequired;
        }
        if (!Enum.IsDefined(typeof(PatientType), patientType))
        {
            return AppointmentErrors.PatientTypeInvalid;
        }

        if (rules is null)
        {
            return AppointmentErrors.AllowedDaysAreRequired;
        }
        
        if(holidays is null)
        {
            return AppointmentErrors.HolidaysAreRequired;
        }

        DateTime baseDate = lastScheduledDate?.Date.AddDays(1) ?? DateTime.UtcNow.Date;

        if (requestedDate.HasValue && requestedDate.Value.Date > baseDate)
        {
            baseDate = requestedDate.Value.Date;
        }

        while (!rules.IsAllowedDay(baseDate.DayOfWeek) || holidays.IsHoliday(baseDate))
        {
            baseDate = baseDate.AddDays(1);
        }

        if (baseDate < DateTime.UtcNow.Date)
            return AppointmentErrors.AttendDateMustBeInFuture;

        return new Appointment(ticketId, patientType, baseDate, AppointmentStatus.Scheduled, notes);
    }
    
    public Result<Updated> Reschedule(DateTime newDate, AppointmentScheduleRules rules,
        HolidayCalendar holidays)
    {
        if (!IsEditable) return AppointmentErrors.Readonly;
        
        if (rules is null)
        {
            return AppointmentErrors.AllowedDaysAreRequired;
        }
        if(holidays is null)
        {
            return AppointmentErrors.HolidaysAreRequired;
        }

        if (!rules.IsAllowedDay(newDate.DayOfWeek))
        {
            return AppointmentErrors.InvalidAppointmentDay(rules.AllowedDays.ToList());
        }
        
        if (holidays.IsHoliday(newDate))
        {
            return AppointmentErrors.AppointmentOnHoliday(newDate);
        }

        if (newDate < DateTime.UtcNow.Date)
            return AppointmentErrors.AttendDateMustBeInFuture;

        AttendDate = newDate;
        return Result.Updated;
    }
  
    public bool IsEditable => Status is AppointmentStatus.Scheduled or AppointmentStatus.Today;
    public bool CanTransitionTo(AppointmentStatus newState)
    {
        return (Status, newState) switch
        {
            (AppointmentStatus.Scheduled, AppointmentStatus.Today) => true,
            (AppointmentStatus.Today, AppointmentStatus.Attended) => true,
            (AppointmentStatus.Today, AppointmentStatus.Absent) => true,
            (_, AppointmentStatus.Cancelled) when Status != AppointmentStatus.Attended => true,
            _ => false
        };
    }

    public Result<Updated> Cancel()
    {
        if (!CanTransitionTo(AppointmentStatus.Cancelled))
        {
            return AppointmentErrors.InvalidStateTransition(Status, AppointmentStatus.Cancelled);
        }

        Status = AppointmentStatus.Cancelled;
        Ticket?.Cancel();
        return Result.Updated;
    }
    public Result<Updated> MarkAsToday()
    {
        if (!CanTransitionTo(AppointmentStatus.Today))
        {
            return AppointmentErrors.InvalidStateTransition(Status, AppointmentStatus.Today);
        }

        if (AttendDate.Date != DateTime.UtcNow.Date)
        {
            return AppointmentErrors.InvalidTodayMark(AttendDate);
        }

        Status = AppointmentStatus.Today;
        Ticket?.Continue();
        return Result.Updated;
    }
    public Result<Updated> MarkAsAttended()
    {
        if (!CanTransitionTo(AppointmentStatus.Attended))
        {
            return AppointmentErrors.InvalidStateTransition(Status, AppointmentStatus.Attended);
        }

        if (AttendDate.Date > DateTime.UtcNow.Date)
        {
            return AppointmentErrors.CannotMarkFutureAsAttended(AttendDate);
        }

        Status = AppointmentStatus.Attended;
        return Result.Updated;
    }

    public Result<Updated> MarkAsAbsent()
    {
        if (!CanTransitionTo(AppointmentStatus.Absent))
        {
            return AppointmentErrors.InvalidStateTransition(Status, AppointmentStatus.Absent);
        }

        if (AttendDate.Date >= DateTime.UtcNow.Date)
        {
            return AppointmentErrors.CannotMarkFutureAsAbsent(AttendDate);
        }

        Status = AppointmentStatus.Absent;
        Ticket?.Cancel();
        return Result.Updated;
    }

    public bool IsAppointmentTomorrow()
    {
        return AttendDate.Date == DateTime.UtcNow.Date.AddDays(1);
    }
}