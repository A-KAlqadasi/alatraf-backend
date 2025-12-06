using AlatrafClinic.Domain.Settings;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IAppSettingRepository : IGenericRepository<AppSetting, int>
{
  Task<AppSetting?> GetByKeyAsync(string key, CancellationToken ct = default);
  Task<string> GetAllowedAppointmentDaysAsync(CancellationToken ct = default);
  
}