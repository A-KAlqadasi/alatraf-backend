using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Tickets;

public static class TicketErrors
{
    public static readonly Error PatientIsRequired = Error.Validation("Ticket.PatientIsRequired", "المريض مطلوب");

    public static readonly Error ServiceIsRequired = Error.Validation("Ticket.ServiceIsRequired", "الخدمة مطلوبة");

    public static readonly Error DiagnosisAlreadyAssigned = Error.Validation("Ticket.DiagnosisAlreadyAssigned", "تم تعيين التشخيص بالفعل لهذه التذكرة");
    public static readonly Error AppointmentAlreadyAssigned = Error.Validation("Ticket.AppointmentAlreadyAssigned", "تم تعيين الموعد بالفعل لهذه التذكرة");
    public static Error InvalidStateTransition(TicketStatus current, TicketStatus next) => Error.Conflict(
       code: "Ticket.InvalidStateTransition",
       description: $" انتقال حالة التذكرة غير صالح من '{current}' إلى '{next}'.");
    public static Error ReadOnly = Error.Conflict(
       code: "Ticket.ReadOnly",
       description: "التذكرة غير قابلة للتعديل");
    public static readonly Error DiagnosisTicketMismatch = Error.Validation("Ticket.DiagnosisTicketMismatch", "التشخيص لا ينتمي إلى هذه التذكرة");
    public static readonly Error AppointmentTicketMismatch = Error.Validation("Ticket.AppointmentTicketMismatch", "الموعد لا ينتمي إلى هذه التذكرة");
    public static readonly Error TicketNotFound = Error.NotFound("Ticket.NotFound", "التذكرة غير موجودة");
    public static readonly Error TicketAlreadHasAppointment = Error.Conflict("Ticket.AlreadHasAppointment", "التذكرة تحتوي بالفعل على موعد");
    public static readonly Error TicketPaused = Error.Conflict("Ticket.Paused", "التذكرة متوقفة ولا يمكن قبول هذه العملية");

    public static readonly Error TicketServiceIsNotRenewal = Error.Conflict("Ticket.TicketServiceIsNotRenewal", "خدمة التذكرة ليست تجديد");
    
}