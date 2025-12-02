using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Departments.Sections;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class SectionRepository : GenericRepository<Section, int>, ISectionRepository
{
    public SectionRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> IsSectionHasRoomNameAsync(int sectionId, string roomName, CancellationToken ct)
    {
        return await dbContext.Sections
            .Where(s => s.Id == sectionId)
            .SelectMany(s => s.Rooms)
            .AnyAsync(r => r.Name == roomName, ct);
    }
}