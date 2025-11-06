using AlatrafClinic.Application.Features.People.Patients.Mappers;
using AlatrafClinic.Application.Features.Services.Mappers;
using AlatrafClinic.Application.Features.Tickets.Dtos;
using AlatrafClinic.Domain.Services.Tickets;

namespace AlatrafClinic.Application.Features.Tickets.Mappers;

public static class TicketMapper
{
    public static TicketDto ToDto(this Ticket ticket)
    {
        return new TicketDto
        {
            Id = ticket.Id,
            Service = ticket.Service?.ToDto(),
            Patient = ticket.Patient?.ToDto(),
            Status = ticket.Status
        };
    }
}