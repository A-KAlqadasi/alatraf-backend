using AlatrafClinic.Domain.RepairCards.Enums;

namespace AlatrafClinic.Application.Features.RepairCards.Dtos;

public class OrderDto
{
    public int Id { get; set; }
    public int? RepairCardId { get; set; }
    public int SectionId { get; set; }
    public OrderType OrderType { get; set; }
    public OrderStatus Status { get; set; }
    public bool IsEditable { get; set; }
}
