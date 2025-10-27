using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.RepairCards.Orders;

namespace AlatrafClinic.Domain.Inventory.Items;


public class Item : AuditableEntity<int>
{
    public string? Name { get; set; }
    public decimal Price { get; set; } // optional baseline; you can keep or drop later
    public int Quantity { get; set; }
    public decimal? MinPriceToPay { get; set; }
    public decimal? MaxPriceToPay { get; set; }
    public int? MinQuantity { get; set; }
    public bool? IsActive { get; set; }
    private readonly List<ItemUnit> _itemUnits = new();
    public IReadOnlyCollection<ItemUnit> ItemUnits => _itemUnits.AsReadOnly();
    public ICollection<OrderItem> OrderItem { get; set; } = new List<OrderItem>();

    // public ICollection<StoreItems> StoreItems { get; set; } = new();
    // public ICollection<PurchaseItem> PurchaseItems { get; set; } = new();
    // public ICollection<SalesItems> SalesItems { get; set; } = new();
}