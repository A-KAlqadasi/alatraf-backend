using AlatrafClinic.Domain.Inventory.Purchases;
using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
namespace AlatrafClinic.Application.Common.Interfaces.Repositories.Inventory;

public interface IPurchaseInvoiceRepository : IGenericRepository<PurchaseInvoice, int>
{
    /// <summary>
    /// Returns a read-only projection of all purchase invoices as DTOs.
    /// Implement this with an efficient EF Core projection in Infrastructure.
    /// </summary>
    Task<IEnumerable<PurchaseInvoiceDto>> GetAllProjectedAsync(CancellationToken cancellationToken = default);
    /// <summary>
    /// Returns read-only projection of items for a given purchase invoice id.
    /// Implement with EF Core projection in Infrastructure to avoid loading aggregates.
    /// </summary>
    Task<IEnumerable<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseItemDto>> GetItemsProjectedAsync(int purchaseInvoiceId, CancellationToken cancellationToken = default);
}
