namespace AlatrafClinic.Domain.Services.Appointments;

public sealed class AppointmentScheduleRules
{
    public IReadOnlyCollection<DayOfWeek> AllowedDays { get; }

    public AppointmentScheduleRules(IEnumerable<DayOfWeek> allowedDays)
    {
        AllowedDays = allowedDays.ToList().AsReadOnly();
    }

    public bool IsAllowedDay(DayOfWeek day)
    {
        return AllowedDays.Contains(day);
    }

    public DayOfWeek GetNextAllowedDay(DayOfWeek current)
    {
        var orderedDays = AllowedDays.OrderBy(d => d).ToList();
        var next = orderedDays.FirstOrDefault(d => d > current);
        return next == default ? orderedDays.First() : next;
    }
}