using System.ComponentModel.DataAnnotations;

using AlatrafClinic.Domain.Services.Enums;

namespace AlatrafClinic.Api.Requests.Holidays;

public sealed record CreateHolidayRequest(
    [Required] DateOnly StartDate,
    DateOnly? EndDate,
    [Required] [MaxLength(100)] string Name,
    bool IsRecurring,
    [Required] HolidayType Type,
    bool IsActive = true);
