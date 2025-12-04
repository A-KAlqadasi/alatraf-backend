namespace AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;

public class PurchaseInvoiceSummaryDto
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string StoreName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalQuantities { get; set; }
    public decimal TotalPrice { get; set; }
}
