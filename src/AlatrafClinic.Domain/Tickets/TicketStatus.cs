namespace AlatrafClinic.Domain.Tickets;

public enum TicketStatus : byte
{
    New = 0,
    Pause,
    Continue,
    Completed,
    Cancelled
}