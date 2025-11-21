using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Enums;

namespace AlatrafClinic.Domain.Services.Appointments;

public static class AppointmentErrors
{
    public static readonly Error AttendDateMustBeInFuture = Error.Validation("Appointment.AttendDateMustBeInFuture", "Appointment date must be in the future.");
    public static Error InvalidAppointmentDay(List<DayOfWeek> allowedDays) => Error.Validation("Appointment.InvalidAppointmentDay", $"Appointments can only be scheduled on {string.Join(", ", allowedDays)}.");
    public static Error AppointmentOnHoliday(DateTime date) => Error.Validation("Appointment.AppointmentOnHoliday", $"Appointments cannot be scheduled on holidays. The date {date:yyyy-MM-dd} is a holiday.");
    
    public static readonly Error AllowedDaysAreRequired = Error.Validation("Appointment.AllowedDaysAreRequired", "Allowed days for scheduling appointments must be provided.");
    public static readonly Error TicketIdRequired = Error.Validation("Appointment.TicketIdRequired", "A valid Ticket Id is required to schedule an appointment.");
    public static Error PatientTypeInvalid =>
       Error.Validation("Appointment.PatientTypeInvalid", "Invalid patient type.");
    public static Error InvalidStateTransition(AppointmentStatus current, AppointmentStatus next) => Error.Conflict(
       code: "Appointment.InvalidStateTransition",
       description: $"Appointment Invalid State transition from '{current}' to '{next}'.");
    public static Error InvalidTodayMark(DateTime attendDate) => Error.Conflict(
       code: "Appointment.InvalidTodayMark", description: $"Cannot mark appointment as 'Today' when its attend date is '{attendDate:yyyy-MM-dd}'.");
    public static Error CannotMarkFutureAsAttended(DateTime attendDate) => Error.Conflict(
       code: "Appointment.CannotMarkFutureAsAttended", description: $"Cannot mark appointment as 'Attended' when its attend date is in the future '{attendDate:yyyy-MM-dd}'.");
    public static Error CannotMarkFutureAsAbsent(DateTime attendDate) => Error.Conflict(
       code: "Appointment.CannotMarkFutureAsAbsent", description: $"Cannot mark appointment as 'Absent' when its attend date is in the future '{attendDate:yyyy-MM-dd}'.");
    public static Error Readonly => Error.Conflict(
    code: "Appointment.Readonly",
    description: "Appointment is read-only.");

    public static Error HolidaysAreRequired = Error.Validation("Appointment.HolidaysAreRequired", "Holiday calendar must be provided.");
    public static readonly Error AppointmentNotFound = Error.NotFound("Appointment.NotFound", "Appointment not found.");
}