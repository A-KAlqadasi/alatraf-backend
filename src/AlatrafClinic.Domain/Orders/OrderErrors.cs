using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Orders;

public static class OrderErrors
{
    public static readonly Error InvalidSection     = Error.Validation("Order.InvalidSection", "القسم المحدد غير صالح لهذا النوع من الطلبات.");
    public static readonly Error InvalidRepairCard  = Error.Validation("Order.InvalidRepairCard", "بطاقة الإصلاح مطلوبة لهذا النوع من الطلبات.");
    public static readonly Error NoItems            = Error.Validation("Order.NoItems", "مطلوب عنصر طلب واحد على الأقل.");
    public static readonly Error ReadOnly = Error.Validation("Order.ReadOnly", "الطلب غير قابل للتعديل في الحالة الحالية.");
    
    public static readonly Error OrderNotFound = Error.NotFound("Order.NotFound", "الطلب غير موجود.");
}