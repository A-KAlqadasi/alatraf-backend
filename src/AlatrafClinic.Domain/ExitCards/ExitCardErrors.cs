using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.ExitCards;

public static class ExitCardErrors
{
    public static readonly Error PatientIdIsRequired = Error.Validation("ExitCard.PatientIdIsRequired", "معرف المريض مطلوب");
    public static readonly Error RepairCardIsRequired = Error.Validation("ExitCard.RepairCardIsRequired", "بطاقة الإصلاح مطلوبة");
    public static readonly Error SaleIsRequired = Error.Validation("ExitCard.SaleIsRequired", "المبيعات مطلوبة");
    public static readonly Error AlreadyAssignedToSale = Error.Conflict("ExitCard.AlreadyAssignedToSale", "تم تعيين بطاقة الخروج بالفعل لمبيعات");
    public static readonly Error AlreadyAssignedToRepairCard = Error.Conflict("ExitCard.AlreadyAssignedToRepairCard", "تم تعيين بطاقة الخروج بالفعل لبطاقة إصلاح");
}