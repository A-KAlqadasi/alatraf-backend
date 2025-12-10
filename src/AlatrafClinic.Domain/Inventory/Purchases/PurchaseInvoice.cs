using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Enums;
using AlatrafClinic.Domain.Inventory.Stores;
using AlatrafClinic.Domain.Inventory.Suppliers;

namespace AlatrafClinic.Domain.Inventory.Purchases;

public class PurchaseInvoice : AuditableEntity<int>
{
    // public int PurchaseInvoiceId { get; protected set; }
    public string Number { get; private set; } = string.Empty;
    public DateTime Date { get; private set; }

    public int SupplierId { get; private set; }
    public Supplier Supplier { get; private set; } = default!;

    public int StoreId { get; private set; }
    public Store Store { get; private set; } = default!;

    public PurchaseInvoiceStatus Status { get; private set; } = PurchaseInvoiceStatus.Draft;
    public DateTime? PostedAtUtc { get; private set; }
    public DateTime? PaidAtUtc { get; private set; }
    public decimal? PaymentAmount { get; private set; }
    public string? PaymentMethod { get; private set; }
    public string? PaymentReference { get; private set; }

    private readonly List<PurchaseItem> _items = new();
    public IReadOnlyCollection<PurchaseItem> Items => _items.AsReadOnly();

    public decimal TotalQuantities => _items.Sum(i => i.Quantity);
    public decimal TotalPrice => _items.Sum(i => i.Total);

    private PurchaseInvoice() { }

    private PurchaseInvoice(string number, DateTime date, Supplier supplier, Store store)
    {
        Number = number;
        Date = date;
        Supplier = supplier;
        SupplierId = supplier.Id;
        Store = store;
        StoreId = store.Id;
    }

    public static Result<PurchaseInvoice> Create(string number, DateTime date, Supplier supplier, Store store)
    {
        if (string.IsNullOrWhiteSpace(number)) return PurchaseInvoiceErrors.NumberRequired;
        if (supplier is null) return PurchaseInvoiceErrors.InvalidSupplier;
        if (store is null) return PurchaseInvoiceErrors.InvalidStore;

        return new PurchaseInvoice(number, date, supplier, store); ;
    }

    public Result<Updated> UpdateHeader(string number, DateTime date, Supplier supplier, Store store)
    {
        if (Status != PurchaseInvoiceStatus.Draft) return PurchaseInvoiceErrors.NotDraft;

        if (string.IsNullOrWhiteSpace(number)) return PurchaseInvoiceErrors.NumberRequired;
        if (supplier is null) return PurchaseInvoiceErrors.InvalidSupplier;
        if (store is null) return PurchaseInvoiceErrors.InvalidStore;

        // If store changes, all existing items must match new store (or clear first)
        if (_items.Any() && store.Id != StoreId)
            return PurchaseInvoiceErrors.MixedStores;

        Number = number;
        Date = date;
        Supplier = supplier;
        SupplierId = supplier.Id;
        Store = store;
        StoreId = store.Id;

        return Result.Updated;
    }

    /// <summary>
    /// Add or increase a line (merged by StoreItemUnitId).
    /// </summary>
    public Result<Updated> AddItem(StoreItemUnit storeItemUnit, decimal quantity, decimal unitPrice, string? notes = null)
    {
        if (Status != PurchaseInvoiceStatus.Draft) return PurchaseInvoiceErrors.NotDraft;
        if (storeItemUnit is null) return PurchaseItemErrors.InvalidItem;
        if (storeItemUnit.StoreId != StoreId) return PurchaseItemErrors.WrongStore;
        if (quantity <= 0) return PurchaseItemErrors.InvalidQuantity;
        if (unitPrice <= 0) return PurchaseItemErrors.InvalidUnitPrice;

        var existing = _items.FirstOrDefault(i => i.StoreItemUnitId == storeItemUnit.Id);
        if (existing is not null)
        {
            // Merge by increasing quantity; keep last unit price (or choose to weighted-average if desired)
            existing.IncreaseQuantity(quantity);
            return Result.Updated;
        }

        var created = PurchaseItem.Create(this.Id, storeItemUnit, quantity, unitPrice, notes);
        if (created.IsError) return created.Errors;

        // Set back-reference
        var line = created.Value;

        line.AssignPurchaseInvoice(this);

        _items.Add(line);
        return Result.Updated;
    }

    /// <summary>
    /// Replace all items with a new set (validates same store and merges duplicates).
    /// </summary>
    public Result<Updated> ReplaceItems(IEnumerable<PurchaseItem> newItems)
    {
        if (Status != PurchaseInvoiceStatus.Draft) return PurchaseInvoiceErrors.NotDraft;

        var list = (newItems ?? Enumerable.Empty<PurchaseItem>()).ToList();
        if (list.Count == 0) return PurchaseInvoiceErrors.ItemsRequired;

        // Validate store matching and line values
        foreach (var it in list)
        {
            if (it.StoreItemUnit is null) return PurchaseItemErrors.InvalidItem;
            if (it.StoreItemUnit.StoreId != StoreId) return PurchaseItemErrors.WrongStore;
            if (it.Quantity <= 0) return PurchaseItemErrors.InvalidQuantity;
            if (it.UnitPrice <= 0) return PurchaseItemErrors.InvalidUnitPrice;
        }

        // Merge duplicates by StoreItemUnitId
        var merged = new Dictionary<int, PurchaseItem>();
        foreach (var it in list)
        {
            if (merged.TryGetValue(it.StoreItemUnitId, out var existing))
            {
                existing.IncreaseQuantity(it.Quantity);
            }
            else
            {
                it.AssignPurchaseInvoice(this);

                merged.Add(it.StoreItemUnitId, it);
            }
        }

        _items.Clear();
        _items.AddRange(merged.Values);

        return Result.Updated;
    }

    public Result<Updated> UpdateItem(int storeItemUnitId, StoreItemUnit newStoreItemUnit, decimal quantity, decimal unitPrice, string? notes = null)
    {
        if (Status != PurchaseInvoiceStatus.Draft) return PurchaseInvoiceErrors.NotDraft;

        var existing = _items.FirstOrDefault(i => i.StoreItemUnitId == storeItemUnitId);
        if (existing is null) return Result.Updated; // no-op

        if (newStoreItemUnit.StoreId != StoreId) return PurchaseItemErrors.WrongStore;

        var result = existing.Update(this.Id, newStoreItemUnit, quantity, unitPrice, notes);
        if (result.IsError) return result.Errors;

        // If StoreItemUnitId changed and causes a duplicate, merge
        var duplicate = _items.FirstOrDefault(i => i != existing && i.StoreItemUnitId == existing.StoreItemUnitId);
        if (duplicate is not null)
        {
            duplicate.IncreaseQuantity(existing.Quantity);
            _items.Remove(existing);
        }

        return Result.Updated;
    }

    public Result<Updated> RemoveItem(int storeItemUnitId)
    {
        if (Status != PurchaseInvoiceStatus.Draft) return PurchaseInvoiceErrors.NotDraft;
        _items.RemoveAll(i => i.StoreItemUnitId == storeItemUnitId);
        return Result.Updated;
    }

    public Result<Updated> ClearItems()
    {
        if (Status != PurchaseInvoiceStatus.Draft) return PurchaseInvoiceErrors.NotDraft;
        _items.Clear();
        return Result.Updated;
    }

    /// <summary>
    /// Applies inventory movement: increases StoreItemUnit quantities and locks the invoice.
    /// </summary>
    public Result<Updated> Post()
    {
        if (Status == PurchaseInvoiceStatus.Posted) return PurchaseInvoiceErrors.AlreadyPosted;
        if (Status == PurchaseInvoiceStatus.Cancelled) return PurchaseInvoiceErrors.AlreadyCancelled;
        if (_items.Count == 0) return PurchaseInvoiceErrors.ItemsRequired;

        // Final store consistency check
        if (_items.Any(i => i.StoreItemUnit.StoreId != StoreId))
            return PurchaseInvoiceErrors.MixedStores;

        foreach (var line in _items)
        {
            var inc = line.StoreItemUnit.Increase(line.Quantity);
            if (inc.IsError) return inc.Errors;
        }

        Status = PurchaseInvoiceStatus.Posted;
        PostedAtUtc = DateTime.UtcNow;

        return Result.Updated;
    }

    /// <summary>
    /// Reverts inventory movement of a posted invoice.
    /// </summary>
    public Result<Updated> Unpost()
    {
        if (Status != PurchaseInvoiceStatus.Posted) return PurchaseInvoiceErrors.NotPosted;

        foreach (var line in _items)
        {
            var dec = line.StoreItemUnit.Decrease(line.Quantity);
            if (dec.IsError) return dec.Errors;
        }

        Status = PurchaseInvoiceStatus.Draft;
        PostedAtUtc = null;
        return Result.Updated;
    }

    public Result<Updated> Cancel()
    {
        if (Status == PurchaseInvoiceStatus.Cancelled) return PurchaseInvoiceErrors.AlreadyCancelled;
        if (Status == PurchaseInvoiceStatus.Posted || Status == PurchaseInvoiceStatus.Paid) return PurchaseInvoiceErrors.CannotCancelPostedOrPaid;

        Status = PurchaseInvoiceStatus.Cancelled;
        return Result.Updated;
    }

    public Result<Updated> MarkPaid(decimal amount, string paymentMethod, string? paymentReference = null)
    {
        if (Status == PurchaseInvoiceStatus.Paid) return PurchaseInvoiceErrors.AlreadyPaid;
        if (Status == PurchaseInvoiceStatus.Cancelled) return PurchaseInvoiceErrors.AlreadyCancelled;
        if (Status != PurchaseInvoiceStatus.Posted) return PurchaseInvoiceErrors.NotPosted;

        if (amount <= 0) return AlatrafClinic.Domain.Inventory.Purchases.PurchaseItemErrors.InvalidQuantity; // reuse an error? better create Payment errors but keep simple

        Status = PurchaseInvoiceStatus.Paid;
        PaidAtUtc = DateTime.UtcNow;
        PaymentAmount = amount;
        PaymentMethod = paymentMethod;
        PaymentReference = paymentReference;

        return Result.Updated;
    }
}