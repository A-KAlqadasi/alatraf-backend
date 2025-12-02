using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Departments.Sections.Rooms;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class RoomRepository : GenericRepository<Room, int>, IRoomRepository
{
    public RoomRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }
}