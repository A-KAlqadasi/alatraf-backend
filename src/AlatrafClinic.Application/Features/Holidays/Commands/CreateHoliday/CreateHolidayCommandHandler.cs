using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Holidays.Dtos;
using AlatrafClinic.Application.Features.Holidays.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Appointments.Holidays;
using AlatrafClinic.Domain.Services.Enums;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Holidays.Commands.CreateHoliday;

public class CreateHolidayCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<CreateHolidayCommandHandler> logger,
    ICacheService cache
) : IRequestHandler<CreateHolidayCommand, Result<HolidayDto>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
  private readonly ILogger<CreateHolidayCommandHandler> _logger = logger;
  private readonly ICacheService _cache = cache;

  public async Task<Result<HolidayDto>> Handle(CreateHolidayCommand req, CancellationToken ct)
  {

    var alreadyExists = await _unitOfWork.Holidays.HasSameHoliday(req.StartDate, ct);

    if (alreadyExists)
    {
      return ApplicationErrors.HolidayAlreadyExists(req.StartDate);

    }


    Result<Holiday> holidayResult;

    if (req.Type == HolidayType.Fixed)
    {
      holidayResult = Holiday.CreateFixed(req.StartDate, req.Name);
    }
    else
    {
      holidayResult = Holiday.CreateTemporary(
          req.StartDate,
          req.Name,
          req.EndDate
      );
    }

    if (holidayResult.IsError)
      return holidayResult.Errors;

    var holiday = holidayResult.Value;

    if (req.IsActive)
      holiday.Activate();
    else
      holiday.Deactivate();

    await _unitOfWork.Holidays.AddAsync(holiday, ct);
    await _unitOfWork.SaveChangesAsync(ct);

    _logger.LogInformation("Holiday created successfully with ID: {id}", holiday.Id);

    await _cache.RemoveByTagAsync("holidays", ct);

    return holiday.ToDto();
  }
}