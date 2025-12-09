using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Services;
using AlatrafClinic.Domain.Services.Tickets;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class TicketRepository : GenericRepository<Ticket, int>, ITicketRepository
{
    
    public TicketRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }

    public async new Task<Ticket?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await dbContext.Set<Ticket>().Include(t => t.Patient!).ThenInclude(t=> t.Person)
                                        .Include(t => t.Service)
                                        .ThenInclude(s => s.Department)  
                                        .FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task<Service?> GetTicketServiceAsync(int ticketId, CancellationToken ct = default)
    {
       var ticket = await dbContext.Tickets.Include(t=> t.Service).FirstOrDefaultAsync(t=> t.Id == ticketId, ct);

       return ticket?.Service;
    }
}