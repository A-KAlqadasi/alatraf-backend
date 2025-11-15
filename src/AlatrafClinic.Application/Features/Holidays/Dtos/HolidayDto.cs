using AlatrafClinic.Domain.Services.Enums;

namespace AlatrafClinic.Application.Features.Holidays.Dtos;
public sealed class HolidayDto
{
    public int HolidayId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Name { get; set; }
    public bool IsRecurring { get; set; }
    public bool IsActive { get; set; }
    public HolidayType Type { get; set; }  
}