namespace AlatrafClinic.Domain.Orders.Enums;

public enum OrderStatus : byte
{
    Draft = 0,
    Posted = 1,
    Cancelled = 2,
}