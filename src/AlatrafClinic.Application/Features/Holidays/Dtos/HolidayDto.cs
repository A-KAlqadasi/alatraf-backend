using AlatrafClinic.Domain.Holidays;

namespace AlatrafClinic.Application.Features.Holidays.Dtos;
public sealed class HolidayDto
{
    public int HolidayId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Name { get; set; }
    public bool IsRecurring { get; set; }
    public bool IsActive { get; set; }
    public HolidayType Type { get; set; }  
}