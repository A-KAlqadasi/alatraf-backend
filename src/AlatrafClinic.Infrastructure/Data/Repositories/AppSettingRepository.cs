using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Settings;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class AppSettingRepository : GenericRepository<AppSetting, int>, IAppSettingRepository
{
    public AppSettingRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<AppSetting?> GetByKeyAsync(string key, CancellationToken ct = default)
    {
        return await dbContext.AppSettings
            .FirstOrDefaultAsync(a => a.Key == key, ct);
    }
}