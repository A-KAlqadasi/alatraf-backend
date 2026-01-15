using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Appointments;

public static class AppointmentErrors
{
    public static readonly Error AttendDateMustBeInFuture = Error.Conflict("Appointment.AttendDateMustBeInFuture", "تاريخ الحضور يجب أن يكون في المستقبل");
    public static readonly Error TicketIdRequired = Error.Validation("Appointment.TicketIdRequired", "رقم التذكرة مطلوب");
    public static Error PatientTypeInvalid =>
       Error.Validation("Appointment.PatientTypeInvalid", "نوع المريض غير صالح");
    public static Error InvalidStateTransition(AppointmentStatus current, AppointmentStatus next) => Error.Conflict(
       code: "Appointment.InvalidStateTransition",
       description: $"تغيير حالة الموعد غير صالح من '{current}' إلى '{next}'");
    public static Error InvalidTodayMark(DateOnly attendDate) => Error.Conflict(
       code: "Appointment.InvalidTodayMark", description: $"لا يمكن وضع علامة 'اليوم' على الموعد لان موعد الحضور مختلف '{attendDate:yyyy-MM-dd}'");
    public static Error CannotMarkFutureAsAttended(DateOnly attendDate) => Error.Conflict(
       code: "Appointment.CannotMarkFutureAsAttended", description: $"لا يمكن وضع علامة 'حضر' على الموعد عندما يكون تاريخ الحضور في المستقبل '{attendDate:yyyy-MM-dd}'");
    public static Error CannotMarkFutureAsAbsent(DateOnly attendDate) => Error.Conflict(
       code: "Appointment.CannotMarkFutureAsAbsent", description: $"لا يمكن وضع علامة 'غائب' على الموعد عندما يكون تاريخ الحضور في المستقبل '{attendDate:yyyy-MM-dd}'");
    public static Error Readonly => Error.Conflict(
    code: "Appointment.Readonly",
    description:"لا يمكن تعديل هذا الموعد لأنه في حالة غير قابلة للتعديل");
    public static readonly Error AppointmentNotFound = Error.NotFound("Appointment.NotFound", "الموعد غير موجود");
}