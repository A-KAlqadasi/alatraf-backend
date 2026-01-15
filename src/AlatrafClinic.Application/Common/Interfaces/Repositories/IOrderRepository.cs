using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Domain.Orders;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IOrderRepository : IGenericRepository<Order, int>
{
    // Projection for order items to avoid loading the entire aggregate in read scenarios
    Task<IEnumerable<OrderItemDto>> GetItemsProjectedAsync(int orderId, CancellationToken ct = default);

    // Projection to fetch orders filtered by section (read-only DTOs)
    Task<IEnumerable<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>> GetBySectionProjectedAsync(int sectionId, CancellationToken ct = default);

    // Projection to fetch draft orders (read-only DTOs)
    Task<IEnumerable<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>> GetDraftsProjectedAsync(CancellationToken ct = default);
    // Queryable for advanced read scenarios (filtering, paging, sorting, search)
    Task<IQueryable<Order>> GetOrdersQueryAsync(CancellationToken ct = default);
}
