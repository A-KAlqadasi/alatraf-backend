using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.RepairCards.DeliveryTimes;

public static class DeliveryTimeErrors
{
    public static readonly Error DeliveryTimeInPast = Error.Validation("DeliveryTime.InPast", "وقت التسليم لا يمكن أن يكون في الماضي");
    public static readonly Error RepairCardIsRequired = Error.Validation("DeliveryTime.RepairCardIsRequired", "بطاقة الإصلاح مطلوبة");
}