using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Orders;

public static class OrderItemErrors
{
    public static readonly Error InvalidQuantity = Error.Validation("OrderItem.InvalidQuantity", "الكمية يجب أن تكون أكبر من صفر.");
    public static readonly Error OrderIdIsRequired = Error.Validation("OrderItem.OrderIdIsRequired", "معرف الطلب مطلوب");
    public static readonly Error ItemUnitRequired = Error.Validation("OrderItem.ItemUnitRequired", "وحدة العنصر مطلوبة.");
  

}