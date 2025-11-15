using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Holidays.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Holidays.Commands.UpdateHoliday;

public class UpdateHolidayCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<UpdateHolidayCommandHandler> logger,
    ICacheService cache
) : IRequestHandler<UpdateHolidayCommand, Result<Updated>>
{
  private readonly IUnitOfWork _uow = unitOfWork;
  private readonly ILogger<UpdateHolidayCommandHandler> _logger = logger;
  private readonly ICacheService _cache = cache;

  public async Task<Result<Updated>> Handle(UpdateHolidayCommand req, CancellationToken ct)
  {
    // 1️⃣ Get the holiday from repository
    var holiday = await _uow.Holidays.GetByIdAsync(req.HolidayId, ct);
    if (holiday is null)
      return ApplicationErrors.HolidayNotFound;
    holiday.UpdateHoliday(
        name: req.Name,
        startDate: req.StartDate,
        endDate: req.EndDate,
        isRecurring: req.IsRecurring,
        type: req.Type
    );

    if (req.IsActive)
      holiday.Activate();
    else
      holiday.Deactivate();

    await _uow.Holidays.UpdateAsync(holiday, ct);
    await _uow.SaveChangesAsync(ct);

    _logger.LogInformation("Holiday updated successfully with ID: {id}", holiday.Id);

    await _cache.RemoveByTagAsync("holidays", ct);

    return Result.Updated;
  }
}