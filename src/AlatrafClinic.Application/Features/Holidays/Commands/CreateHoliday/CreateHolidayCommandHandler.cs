using AlatrafClinic.Application.Common.Errors;
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Holidays.Dtos;
using AlatrafClinic.Application.Features.Holidays.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Holidays;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Holidays.Commands.CreateHoliday;

public class CreateHolidayCommandHandler(
    IAppDbContext _context,
    ILogger<CreateHolidayCommandHandler> _logger,
    HybridCache _cache
) : IRequestHandler<CreateHolidayCommand, Result<HolidayDto>>
{


    public async Task<Result<HolidayDto>> Handle(CreateHolidayCommand command, CancellationToken ct)
    {

        var alreadyExists = await _context.Holidays
            .AnyAsync(h => h.StartDate == command.StartDate, ct);

        if (alreadyExists)
        {
            _logger.LogWarning("Holiday with this date: {startDate} already exists", command.StartDate);
            return ApplicationErrors.HolidayAlreadyExists(command.StartDate);
        }


        Result<Holiday> holidayResult;

        if (command.Type == HolidayType.Fixed)
        {
            holidayResult = Holiday.CreateFixed(command.StartDate, command.Name);
        }
        else
        {
            holidayResult = Holiday.CreateTemporary(
                command.StartDate,
                command.Name,
                command.EndDate);
        }

        if (holidayResult.IsError)
            return holidayResult.Errors;

        var holiday = holidayResult.Value;

        if (command.IsActive)
            holiday.Activate();
        else
            holiday.Deactivate();

        await _context.Holidays.AddAsync(holiday, ct);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("holiday", ct);

        _logger.LogInformation("Holiday created successfully with ID: {id}", holiday.Id);

        return holiday.ToDto();
    }
}