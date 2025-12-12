using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.RepairCards.DeliveryTimes;

public class DeliveryTime : AuditableEntity<int>
{
    public DateOnly DeliveryDate { get; private set; }
    public string? Note { get; private set; }
    public int RepairCardId { get; private set; }
    public RepairCard? RepairCard { get; set; }

    private DeliveryTime() { }
    private DeliveryTime(int repairCardId, DateOnly deliveryDate, string? note)
    {
        RepairCardId = repairCardId;
        DeliveryDate = deliveryDate;
        Note = note;
    }
    public static Result<DeliveryTime> Create(int repairCardId, DateOnly deliveryDate, string? note)
    {
        if (repairCardId <= 0)
        {
            return DeliveryTimeErrors.RepairCardIsRequired;
        }
        if (deliveryDate < AlatrafClinicConstants.TodayDate)
        {
            return DeliveryTimeErrors.DeliveryTimeInPast;
        }

        return new DeliveryTime(repairCardId, deliveryDate, note);
    }
}
