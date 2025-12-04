using AlatrafClinic.Domain.Inventory.Purchases;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;

public class PurchaseInvoiceDto
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public int StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? PostedAtUtc { get; set; }
    public DateTime? PaidAtUtc { get; set; }
    public decimal? PaymentAmount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentReference { get; set; }
    public decimal TotalQuantities { get; set; }
    public decimal TotalPrice { get; set; }
}
