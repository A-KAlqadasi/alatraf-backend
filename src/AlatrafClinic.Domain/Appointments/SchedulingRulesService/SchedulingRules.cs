using AlatrafClinic.Domain.Holidays;

namespace AlatrafClinic.Domain.Appointments.SchedulingRulesService;


public interface ISchedulingRulesProvider
{
    Task<SchedulingRules> GetSchedulingRulesAsync(CancellationToken ct);
}

public record SchedulingRules(
    IReadOnlyCollection<DayOfWeek> AllowedDays,
    IReadOnlyCollection<Holiday> Holidays,
    int DailyCapacity
);

public sealed class SchedulingContext
{
    public DateOnly Today { get; }
    public DateOnly? LastAppointmentDate { get; }
    public DateOnly StartDate { get; }
    public SchedulingRules Rules { get; }
    
    private SchedulingContext(DateOnly today, DateOnly? lastAppointmentDate, DateOnly startDate, SchedulingRules rules)
    {
        Today = today;
        LastAppointmentDate = lastAppointmentDate;
        StartDate = startDate;
        Rules = rules;
    }
    
    public static SchedulingContext Create(
        DateOnly today,
        DateOnly? lastAppointmentDate,
        SchedulingRules rules)
    {
        var startDate = DetermineStartDate(today, lastAppointmentDate);
        return new SchedulingContext(today, lastAppointmentDate, startDate, rules);
    }
    
    private static DateOnly DetermineStartDate(DateOnly today, DateOnly? lastAppointmentDate)
    {
        if (!lastAppointmentDate.HasValue)
            return today;
        
        return today > lastAppointmentDate.Value ? today : lastAppointmentDate.Value;
    }
}