using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Settings;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class AppSettingRepository : GenericRepository<AppSetting, int>, IAppSettingRepository
{
    public AppSettingRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }

    public Task<string> GetAllowedAppointmentDaysAsync(CancellationToken ct = default)
    {
        return dbContext.AppSettings
            .Where(a => a.Key == AlatrafClinicConstants.AllowedDaysKey)
            .Select(a => a.Value)
            .FirstOrDefaultAsync(ct)!;
    }

    public async Task<AppSetting?> GetByKeyAsync(string key, CancellationToken ct = default)
    {
        return await dbContext.AppSettings
            .FirstOrDefaultAsync(a => a.Key == key, ct);
    }
}