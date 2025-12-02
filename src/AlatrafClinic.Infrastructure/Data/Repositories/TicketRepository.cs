using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Services.Tickets;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class TicketRepository : GenericRepository<Ticket, int>, ITicketRepository
{
    public TicketRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }
}