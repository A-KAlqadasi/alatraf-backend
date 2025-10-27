using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Inventory.Units;

namespace AlatrafClinic.Domain.Inventory.Items;

public class ItemUnit : AuditableEntity<int>
{
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public int UnitId { get; set; }
    public Unit? Unit { get; set; }
    public decimal PricePerUnit { get; set; }
    public decimal QuantityInUnit { get; set; }
}