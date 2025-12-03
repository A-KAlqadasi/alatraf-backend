using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.RepairCards.DeliveryTimes;

public class DeliveryTime : AuditableEntity<int>
{
    public DateTime DeliveryDate { get; private set; }
    public string? Note { get; private set; }
    public int RepairCardId { get; private set; }
    public RepairCard? RepairCard { get; set; }

    private DeliveryTime() { }
    private DeliveryTime(int repairCardId, DateTime deliveryDate, string? note)
    {
        RepairCardId = repairCardId;
        DeliveryDate = deliveryDate;
        Note = note;
    }
    public static Result<DeliveryTime> Create(int repairCardId, DateTime deliveryDate, string? note)
    {
        if (repairCardId <= 0)
        {
            return DeliveryTimeErrors.RepairCardIsRequired;
        }
        if (deliveryDate.Date < DateTime.Now.Date)
        {
            return DeliveryTimeErrors.DeliveryTimeInPast;
        }

        return new DeliveryTime(repairCardId, deliveryDate, note);
    }
}
