using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.RepairCards.DeliveryTimes;

public static class DeliveryTimeErrors
{
    public static readonly Error DeliveryTimeInPast = Error.Validation("DeliveryTime.InPast", "Delivery time must be in future");
    public static readonly Error RepairCardIsRequired = Error.Validation("DeliveryTime.RepairCardIsRequired", "Repair card is required");
}