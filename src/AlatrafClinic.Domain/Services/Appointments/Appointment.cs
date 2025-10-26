using System.Data;

using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Enums;
using AlatrafClinic.Domain.Services.Appointments.Enums;
using AlatrafClinic.Domain.Services.Appointments.Holidays;
using AlatrafClinic.Domain.Services.Tickets;

namespace AlatrafClinic.Domain.Services.Appointments;

public class Appointment : AuditableEntity<int>
{
    public PatientType? PatientType { get; set; }
    public DateTime AttendDate { get; set; }
    public AppointmentState State { get; set; } 
    public string? Notes { get; set; }
    public int? TicketId { get; set; }
    public Ticket? Ticket { get; set; }
    public static IReadOnlyCollection<DayOfWeek>? AllowedDays { get; private set; }

    private Appointment() { }

    private Appointment(
        PatientType patientType,
        DateTime attendDate,
        AppointmentState state,
        string? notes,
        int? ticketId)
    {
        PatientType = patientType;
        AttendDate = attendDate;
        State = state;
        Notes = notes;
        TicketId = ticketId;
    }

    public static Result<Appointment> Schedule(
        PatientType patientType,
        DateTime? requestedDate,
        string? notes,
        int ticketId,
        DateTime? lastScheduledDate,
        AppointmentScheduleRules rules,
        HolidayCalendar holidays)
    {
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

        if (ticketId <= 0)
        {
            return AppointmentErrors.TicketIdRequired;
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

        return new Appointment(patientType, baseDate, AppointmentState.Scheduled, notes, ticketId);
    }
    
    public Result<Updated> Reschedule(DateTime newDate, AppointmentScheduleRules rules,
        HolidayCalendar holidays)
    {
        if (!IsEditable)
            return AppointmentErrors.Readonly;
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
  
    public bool IsEditable => State is AppointmentState.Scheduled or AppointmentState.Today;
    public bool CanTransitionTo(AppointmentState newState)
    {
        return (State, newState) switch
        {
            (AppointmentState.Scheduled, AppointmentState.Today) => true,
            (AppointmentState.Today, AppointmentState.Attended) => true,
            (AppointmentState.Today, AppointmentState.Absent) => true,
            (_, AppointmentState.Cancelled) when State != AppointmentState.Attended => true,
            _ => false
        };
    }

    public Result<Updated> Cancel()
    {
        if (!CanTransitionTo(AppointmentState.Cancelled))
        {
            return AppointmentErrors.InvalidStateTransition(State, AppointmentState.Cancelled);
        }

        State = AppointmentState.Cancelled;
        return Result.Updated;
    }
    public Result<Updated> MarkAsToday()
    {
        if (!CanTransitionTo(AppointmentState.Today))
        {
            return AppointmentErrors.InvalidStateTransition(State, AppointmentState.Today);
        }

        if (AttendDate.Date != DateTime.UtcNow.Date)
        {
            return AppointmentErrors.InvalidTodayMark(AttendDate);
        }

        State = AppointmentState.Today;
        return Result.Updated;
    }
    public Result<Updated> MarkAsAttended()
    {
        if (!CanTransitionTo(AppointmentState.Attended))
        {
            return AppointmentErrors.InvalidStateTransition(State, AppointmentState.Attended);
        }

        if (AttendDate.Date > DateTime.UtcNow.Date)
        {
            return AppointmentErrors.CannotMarkFutureAsAttended(AttendDate);
        }

        State = AppointmentState.Attended;
        return Result.Updated;
    }

    public Result<Updated> MarkAsAbsent()
    {
        if (!CanTransitionTo(AppointmentState.Absent))
        {
            return AppointmentErrors.InvalidStateTransition(State, AppointmentState.Absent);
        }
        
        if (AttendDate.Date >= DateTime.UtcNow.Date)
        {
            return AppointmentErrors.CannotMarkFutureAsAbsent(AttendDate);
        }

        State = AppointmentState.Absent;
        return Result.Updated;
    }

    private static bool IsAllowedDay(DayOfWeek day)
    {
        return AllowedDays!.Contains(day);
    }

}