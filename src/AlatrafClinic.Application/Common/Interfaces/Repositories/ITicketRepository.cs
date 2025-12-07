using AlatrafClinic.Domain.Services;
using AlatrafClinic.Domain.Services.Tickets;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface ITicketRepository : IGenericRepository<Ticket, int>
{
    Task<Service?> GetTicketServiceAsync(int ticketId, CancellationToken ct = default);

}