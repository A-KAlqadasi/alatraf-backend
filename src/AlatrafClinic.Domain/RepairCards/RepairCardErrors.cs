using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards.Enums;

namespace AlatrafClinic.Domain.RepairCards;

public static class RepairCardErrors
{
    public static readonly Error DiagnosisIndustrialPartNotFound = Error.Validation("RepairCard.DiagnosisIndustrialPart", "الجزء الصناعي في التشخيص غير موجود");
    public static readonly Error Readonly = Error.Conflict("RepairCard.Readonly", "بطاقة الإصلاح للقراءة فقط");
    public static Error InvalidStateTransition(RepairCardStatus current, RepairCardStatus next) => Error.Conflict(
       code: "RepairCard.InvalidStateTransition",
       description: $"تحويل حالة بطاقة الإصلاح غير صالح من '{current}' إلى '{next}'.");
    public static readonly Error OrderAlreadyExists = Error.Conflict("RepairCard.OrderAlreadyExists", "الطلب موجود بالفعل في بطاقة الإصلاح.");

    public static readonly Error InvalidDiagnosisId =
        Error.Validation("Entity.InvalidDiagnosisId", "مرجع التشخيص غير صالح.");
    public static readonly Error InvalidOrder = Error.Validation("RepairCard.InvalidOrder", "مرجع الطلب غير صالح.");
    public static readonly Error ExitCardAlreadyAssigned = Error.Validation("RepairCard.ExitCardAlreadyAssigned", "بطاقة الخروج موجودة بالفعل لكرت الاصلاح هذا");
    public static readonly Error RepairCardNotFound = Error.NotFound("RepairCard.NotFound", "بطاقة الإصلاح غير موجودة");
    public static readonly Error InvalidStatus = Error.Validation("RepairCard.InvalidStatus", "الحالة غير صالحة.");
    public static readonly Error PaymentNotFound = Error.NotFound("RepairCard.PaymentNotFound", "الدفع لبطاقة الإصلاح غير موجود");
    public static readonly Error NoRepairCardsForPaitent = Error.NotFound("RepairCard.NoRepairCardsForPaitent", "لا توجد بطاقات إصلاح للمريض");
}