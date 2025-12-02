using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Services.Appointments.Holidays;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class HolidayRepository : GenericRepository<Holiday, int>, IHolidayRepository
{
    public HolidayRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> HasSameHoliday(DateTime startDate, CancellationToken ct)
    {
        return await dbContext.Holidays
            .AnyAsync(h => h.StartDate.Date == startDate.Date, ct);
    }
}