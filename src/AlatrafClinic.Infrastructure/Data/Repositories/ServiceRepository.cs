using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Services;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class ServiceRepository : GenericRepository<Service, int>, IServiceRepository
{
    public ServiceRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}