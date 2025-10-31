namespace AlatrafClinic.Domain.Services.Enums;

public enum TicketStatus : byte
{
    New = 0,
    Pause,
    Continue,
    Completed,
    Cancelled
}