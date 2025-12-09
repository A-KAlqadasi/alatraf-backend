using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Holidays.Commands.UpdateHoliday;

public class UpdateHolidayCommandHandler(
    IAppDbContext _context,
    ILogger<UpdateHolidayCommandHandler> _logger,
    HybridCache _cache
) : IRequestHandler<UpdateHolidayCommand, Result<Updated>>
{
  

    public async Task<Result<Updated>> Handle(UpdateHolidayCommand command, CancellationToken ct)
    {
        
        var holiday = await _context.Holidays.FirstOrDefaultAsync(h=> h.Id == command.HolidayId, ct);

        if (holiday is null)
        {
            _logger.LogError("Holiday with Id {holidayId} is not found", command.HolidayId);
            return ApplicationErrors.HolidayNotFound;
        }

        holiday.UpdateHoliday(
            name: command.Name,
            startDate: command.StartDate,
            endDate: command.EndDate,
            isRecurring: command.IsRecurring,
            type: command.Type
        );

        if (command.IsActive)
            holiday.Activate();
        else
            holiday.Deactivate();

        _context.Holidays.Update(holiday);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("holiday", ct);

        _logger.LogInformation("Holiday updated successfully with ID: {id}", holiday.Id);

        return Result.Updated;
    }
}