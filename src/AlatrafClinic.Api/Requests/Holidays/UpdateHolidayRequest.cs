using System.ComponentModel.DataAnnotations;

using AlatrafClinic.Domain.Services.Enums;

namespace AlatrafClinic.Api.Requests.Holidays;

public sealed record UpdateHolidayRequest(
    [Required] [MaxLength(100)] string Name,
    [Required] DateOnly StartDate,
    DateOnly? EndDate,
    bool IsRecurring,
    [Required] HolidayType Type,
    bool IsActive);