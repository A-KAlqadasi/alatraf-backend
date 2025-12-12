using AlatrafClinic.Application.Features.Holidays.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Enums;
using MediatR;

namespace AlatrafClinic.Application.Features.Holidays.Commands.CreateHoliday;

public sealed record CreateHolidayCommand(
    DateOnly StartDate,
    DateOnly? EndDate,
    string Name,
    bool IsRecurring,
    HolidayType Type,
    bool IsActive
) : IRequest<Result<HolidayDto>>;
