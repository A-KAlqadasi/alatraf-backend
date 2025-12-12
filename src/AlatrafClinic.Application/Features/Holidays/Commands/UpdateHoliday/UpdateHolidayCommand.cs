using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.Holidays.Commands.UpdateHoliday;

public sealed record UpdateHolidayCommand(
    int HolidayId,
    string Name,
    DateOnly StartDate,
    DateOnly? EndDate,
    bool IsRecurring,
    HolidayType Type,
    bool IsActive
) : IRequest<Result<Updated>>;
