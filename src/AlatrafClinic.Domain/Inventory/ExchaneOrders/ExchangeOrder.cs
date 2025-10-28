using AlatrafClinic.Domain.Common;

namespace AlatrafClinic.Domain.Inventory.ExchangeOrders;

public class ExchangeOrder : AuditableEntity<int>
{
    public int PatientId { get; private set; }
    public int StoreId { get; private set; }
}