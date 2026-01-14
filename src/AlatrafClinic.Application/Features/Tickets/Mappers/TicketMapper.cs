using AlatrafClinic.Application.Features.Patients.Mappers;
using AlatrafClinic.Application.Features.Services.Mappers;
using AlatrafClinic.Application.Features.Tickets.Dtos;
using AlatrafClinic.Domain.Tickets;

namespace AlatrafClinic.Application.Features.Tickets.Mappers;

public static class TicketMapper
{
    public static TicketDto ToDto(this Ticket ticket)
    {
        return new TicketDto
        {
            TicketId = ticket.Id,
            Service = ticket.Service?.ToDto(),
            Patient = ticket.Patient?.ToDto(),
            TicketStatus = ticket.Status
        };
    }
    public static List<TicketDto> ToDtos(this IEnumerable<Ticket> tickets)
    {
        return tickets.Select(t => t.ToDto()).ToList();
    }

    public static string ToArabicTicketStatus(this TicketStatus status)
    {
        return status switch
        {
            TicketStatus.New => "جديد",
            TicketStatus.Pause => "مجدولة بالمواعيد",
            TicketStatus.Continue => "مستمر",
            TicketStatus.Completed => "مكتمل",
            TicketStatus.Cancelled => "ملغي",
            _ => "غير معروف"
        };
    }

}