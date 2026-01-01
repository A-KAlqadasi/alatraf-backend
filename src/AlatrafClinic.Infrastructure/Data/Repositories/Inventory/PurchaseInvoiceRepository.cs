using AlatrafClinic.Application.Common.Interfaces.Repositories.Inventory;
using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Domain.Inventory.Purchases;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories.Inventory;

public class PurchaseInvoiceRepository : GenericRepository<PurchaseInvoice, int>, IPurchaseInvoiceRepository
{
    public PurchaseInvoiceRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<PurchaseInvoiceDto>> GetAllProjectedAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.PurchaseInvoices
            .AsNoTracking()
            .Select(pi => new PurchaseInvoiceDto
            {
                Id = pi.Id,
                Number = pi.Number,
                Date = pi.Date,
                SupplierId = pi.SupplierId,
                SupplierName = pi.Supplier.SupplierName,
                StoreId = pi.StoreId,
                StoreName = pi.Store.Name,
                Status = pi.Status.ToString(),
                PostedAtUtc = pi.PostedAtUtc,
                PaidAtUtc = pi.PaidAtUtc,
                PaymentAmount = pi.PaymentAmount,
                PaymentMethod = pi.PaymentMethod,
                PaymentReference = pi.PaymentReference,
                TotalQuantities = pi.Items.Sum(i => i.Quantity),
                TotalPrice = pi.Items.Sum(i => i.Quantity * i.UnitPrice)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PurchaseItemDto>> GetItemsProjectedAsync(int purchaseInvoiceId, CancellationToken cancellationToken = default)
    {
        return await dbContext.PurchaseItems
            .AsNoTracking()
            .Where(i => i.PurchaseInvoiceId == purchaseInvoiceId)
            .Select(i => new PurchaseItemDto
            {
                Id = i.Id,
                StoreItemUnitId = i.StoreItemUnitId,
                ItemId = i.StoreItemUnit.ItemUnit.ItemId,
                ItemName = i.StoreItemUnit.ItemUnit.Item.Name,
                UnitId = i.StoreItemUnit.ItemUnit.UnitId,
                UnitName = i.StoreItemUnit.ItemUnit.Unit.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Total = i.Quantity * i.UnitPrice,
                Notes = i.Notes
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<PurchaseInvoice?> GetByIdWithItemsAsync(int id, CancellationToken ct)
    {
        return await dbContext.PurchaseInvoices
            .Include(p => p.Items)
                .ThenInclude(i => i.StoreItemUnit)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }
}
