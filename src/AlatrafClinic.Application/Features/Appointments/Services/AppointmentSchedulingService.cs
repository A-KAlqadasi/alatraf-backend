using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Domain.Appointments;
using AlatrafClinic.Domain.Appointments.SchedulingRulesService;
using AlatrafClinic.Domain.Common.Constants;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Appointments.Services;

public sealed class AppointmentSchedulingService : ISchedulingRulesProvider
{
    private readonly IAppDbContext _context;
    private readonly ILogger<AppointmentSchedulingService> _logger;
    
    public AppointmentSchedulingService(IAppDbContext context, ILogger<AppointmentSchedulingService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<SchedulingRules> GetSchedulingRulesAsync(CancellationToken ct)
    {
        // Load allowed days
        var allowedDaysString = await _context.AppSettings.AsNoTracking()
            .Where(a => a.Key == AlatrafClinicConstants.AllowedDaysKey)
            .Select(a => a.Value)
            .FirstOrDefaultAsync(ct);
        
        var allowedDays = ParseAllowedDays(allowedDaysString);
        
        // Validate no Friday in allowed days
        if (allowedDays.Contains(DayOfWeek.Friday))
        {
            _logger.LogWarning("Friday found in allowed days configuration. Removing Friday.");
            allowedDays = allowedDays.Where(d => d != DayOfWeek.Friday).ToList();
        }
        
        // Load holidays
        var holidays = await _context.Holidays.AsNoTracking().ToListAsync(ct);
        
        // Load capacity
        var capacityString = await _context.AppSettings.AsNoTracking()
            .Where(a => a.Key == AlatrafClinicConstants.AppointmentDailyCapacityKey)
            .Select(a => a.Value)
            .FirstOrDefaultAsync(ct);
        
        var dailyCapacity = ParseDailyCapacity(capacityString);
        
        return new SchedulingRules(allowedDays, holidays, dailyCapacity);
    }
    
    private static IReadOnlyCollection<DayOfWeek> ParseAllowedDays(string? allowedDaysString)
    {
        if (string.IsNullOrWhiteSpace(allowedDaysString))
            return GetDefaultAllowedDays();
        
        var parts = allowedDaysString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var days = new List<DayOfWeek>(parts.Length);
        
        foreach (var part in parts)
        {
            if (Enum.TryParse<DayOfWeek>(part, ignoreCase: true, out var day))
                days.Add(day);
        }
        
        return days.Count > 0 ? days : GetDefaultAllowedDays();
    }
    
    private static IReadOnlyCollection<DayOfWeek> GetDefaultAllowedDays()
    {
        return new[]
        {
            DayOfWeek.Saturday,
            DayOfWeek.Sunday,
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday
        };
    }
    
    private static int ParseDailyCapacity(string? capacityString)
    {
        if (string.IsNullOrWhiteSpace(capacityString) || 
            !int.TryParse(capacityString, out var capacity) || 
            capacity <= 0)
        {
            return AlatrafClinicConstants.DefaultAppointmentDailyCapacity;
        }
        
        return capacity;
    }
    
    public async Task<DateOnly?> GetLastAppointmentDateAsync(CancellationToken ct)
    {
        return await _context.Appointments.AsNoTracking()
            .Where(a => a.Status != AppointmentStatus.Cancelled && a.Status != AppointmentStatus.Absent)
            .MaxAsync(a => (DateOnly?)a.AttendDate, ct);
    }
    
    public async Task<int> GetAppointmentCountForDateAsync(DateOnly date, int? excludeAppointmentId, CancellationToken ct)
    {
        var query = _context.Appointments.AsNoTracking()
            .Where(a => a.AttendDate == date 
                && a.Status != AppointmentStatus.Cancelled 
                && a.Status != AppointmentStatus.Absent);
        
        if (excludeAppointmentId.HasValue)
        {
            query = query.Where(a => a.Id != excludeAppointmentId.Value);
        }
        
        return await query.CountAsync(ct);
    }
    
    public bool IsValidAppointmentDate(DateOnly date, SchedulingRules rules)
    {
        // Friday is always off
        if (date.DayOfWeek == DayOfWeek.Friday)
            return false;
        
        // Must be in allowed days
        if (!rules.AllowedDays.Contains(date.DayOfWeek))
            return false;
        
        // Must not be a holiday
        if (rules.Holidays.Any(h => h.Matches(date)))
            return false;
        
        return true;
    }
    
    public async Task<DateOnly> GetNextValidDateAsync(
        SchedulingContext context,
        Func<DateOnly, Task<int>> getAppointmentCountForDateAsync,
        int? excludeAppointmentId = null)
    {
        var date = context.StartDate;
        var maxIterations = 365 * 5; // Prevent infinite loop - 5 years max
        
        for (int i = 0; i < maxIterations; i++)
        {
            // Skip invalid dates
            while (!IsValidAppointmentDate(date, context.Rules))
            {
                date = date.AddDays(1);
                if (i++ > maxIterations)
                    throw new InvalidOperationException("Could not find a valid appointment date within 5 years");
            }
            
            // Check capacity
            var appointmentCount = await getAppointmentCountForDateAsync(date);
            
            if (appointmentCount < context.Rules.DailyCapacity)
            {
                return date;
            }
            
            // Day is full, move to next day
            date = date.AddDays(1);
        }
        
        throw new InvalidOperationException("Could not find an available appointment slot within 5 years");
    }
    
    public async Task<DateOnly> GetNextValidDateForDisplayAsync(DateOnly? afterDate, CancellationToken ct)
    {
        var rules = await GetSchedulingRulesAsync(ct);
        var lastAppointmentDate = await GetLastAppointmentDateAsync(ct);
        
        var contextDate = afterDate ?? lastAppointmentDate ?? AlatrafClinicConstants.TodayDate;
        
        var context = SchedulingContext.Create(
            AlatrafClinicConstants.TodayDate,
            contextDate,
            rules);
        
        return await GetNextValidDateAsync(
            context,
            async date => await GetAppointmentCountForDateAsync(date, null, ct));
    }

    public async Task<AppointmentDaySummaryDto> GetLastAppointmentDaySummaryAsync(CancellationToken ct)
    {
        var lastAttendDate = await GetLastAppointmentDateAsync(ct);
        
        if (lastAttendDate.HasValue)
        {
            var count = await GetAppointmentCountForDateAsync(lastAttendDate.Value, null, ct);
            
            return new AppointmentDaySummaryDto(
                lastAttendDate.Value,
                UtilityService.GetDayNameArabic(lastAttendDate.Value),
                count);
        }
        else
        {
            var nextValidDate = await GetNextValidDateForDisplayAsync(null, ct);
            var count = await GetAppointmentCountForDateAsync(nextValidDate, null, ct);
            
            return new AppointmentDaySummaryDto(
                nextValidDate,
                UtilityService.GetDayNameArabic(nextValidDate),
                count);
        }
    }

}
