
using AlatrafClinic.Domain.Sales;
using AlatrafClinic.Domain.Services.Appointments.Holidays;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IHolidayRepository : IGenericRepository<Holiday, int>
{
    Task<IQueryable<Holiday>> GetHolidaysQueryAsync(CancellationToken ct = default);
   Task< bool> HasSameHoliday(DateTime startDate, CancellationToken ct);
}