using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Domain.Orders;

namespace AlatrafClinic.Application.Features.RepairCards.Mappers;

public static class OrderMapper
{
    public static OrderDto ToDto(this Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            RepairCardId = order.RepairCardId,
            SectionId = order.SectionId,
            OrderType = order.OrderType,
            Status = order.Status,
            IsEditable = order.IsEditable
        };
    }
}
