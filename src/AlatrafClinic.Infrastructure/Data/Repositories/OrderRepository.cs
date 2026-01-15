using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Domain.Orders;
using AlatrafClinic.Domain.Orders.Enums;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class OrderRepository : GenericRepository<Order, int>, IOrderRepository
{
    public OrderRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<OrderItemDto>> GetItemsProjectedAsync(int orderId, CancellationToken ct = default)
    {
        return await dbContext.OrderItems
            .AsNoTracking()
            .Where(oi => oi.OrderId == orderId)
            .Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                ItemId = oi.ItemUnit.ItemId,
                ItemName = oi.ItemUnit.Item.Name,
                UnitId = oi.ItemUnit.UnitId,
                UnitName = oi.ItemUnit.Unit.Name,
                Quantity = oi.Quantity,
                UnitPrice = oi.Price,
                Total = oi.Quantity * oi.Price
            })
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>> GetBySectionProjectedAsync(int sectionId, CancellationToken ct = default)
    {
        return await dbContext.Orders
            .AsNoTracking()
            .Where(o => o.SectionId == sectionId)
            .Select(o => new AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto
            {
                Id = o.Id,
                RepairCardId = o.RepairCardId,
                SectionId = o.SectionId,
                Status = o.Status
            })
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>> GetDraftsProjectedAsync(CancellationToken ct = default)
    {
        return await dbContext.Orders
            .AsNoTracking()
            .Where(o => o.Status == OrderStatus.Draft)
            .Select(o => new AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto
            {
                Id = o.Id,
                RepairCardId = o.RepairCardId,
                SectionId = o.SectionId,
                Status = o.Status
            })
            .ToListAsync(ct);
    }

    public Task<IQueryable<Order>> GetOrdersQueryAsync(CancellationToken ct = default)
    {
        IQueryable<Order> query = dbContext.Orders.AsQueryable();
        return Task.FromResult(query);
    }
}
