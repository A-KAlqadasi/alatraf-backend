using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AlatrafClinic.Application.Features.Holidays.Dtos;
using AlatrafClinic.Domain.Services.Appointments.Holidays;

namespace AlatrafClinic.Application.Features.Holidays.Mappers;


public static class HolidayMapper
{
    public static HolidayDto ToDto(this Holiday entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new HolidayDto
        {
            HolidayId = entity.Id,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Name = entity.Name,
            IsRecurring = entity.IsRecurring,
            IsActive = entity.IsActive,
            Type = entity.Type
        };
    }

    public static List<HolidayDto> ToDtos(this IEnumerable<Holiday> entities)
    {
        return [.. entities.Select(e => e.ToDto())];
    }
}