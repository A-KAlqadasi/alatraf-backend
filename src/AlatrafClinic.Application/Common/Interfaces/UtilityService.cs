using System.Globalization;

namespace AlatrafClinic.Application.Common.Interfaces;

public static class UtilityService
{
    
    public static string GenderToArabicString(bool gender)
    {
        return gender ? "ذكر" : "أنثى";
    }
    public static string GetDayNameArabic(DateOnly date)
    {
        // "dddd" format specifier gives the full day name
        return date.ToString("dddd", arabicCulture); 
    }
    public static CultureInfo arabicCulture = new CultureInfo("ar-SA");
    
    public static decimal CalculatePercentage(decimal part, decimal total)
    {
        if (total == 0) return 0;
        return Math.Round((part / total) * 100, 2);
    }
    
    public static string GetFormattedDateInArabic(DateTime date)
    {
        return  $"{GetDayNameArabic(DateOnly.FromDateTime(date.Date))} - " +
                        $"{date.ToString("dd/M/yyyy")} - " + 
                         $"{date.ToString("h:mm tt", arabicCulture)} ";
    }

    public static DurationAr CalculateDurationArabic(
    DateOnly injuryDate,
    DateOnly? until = null)
    {
        var endDate = until ?? DateOnly.FromDateTime(DateTime.Today);
        var startDate = injuryDate;

        if (startDate > endDate)
            throw new ArgumentException("تاريخ الإصابة لا يمكن أن يكون في المستقبل");

        var cursor = startDate;

        int years = 0;
        while (cursor.AddYears(1) <= endDate)
        {
            cursor = cursor.AddYears(1);
            years++;
        }

        int months = 0;
        while (cursor.AddMonths(1) <= endDate)
        {
            cursor = cursor.AddMonths(1);
            months++;
        }

        int remainingDays = endDate.DayNumber - cursor.DayNumber;

        int weeks = remainingDays / 7;
        int days = remainingDays % 7;

        return new DurationAr
        {
            Years = years,
            Months = months,
            Weeks = weeks,
            Days = days
        };
    }


}

public sealed record DurationAr
{
    public int Years { get; init; }
    public int Months { get; init; }
    public int Weeks { get; init; }
    public int Days { get; init; }

    public override string ToString()
    {
        var parts = new List<string>();

        if (Years > 0)  parts.Add($"{Years} سنة");
        if (Months > 0) parts.Add($"{Months} شهر");
        if (Weeks > 0)  parts.Add($"{Weeks} أسبوع");
        if (Days > 0)   parts.Add($"{Days} يوم");

        return parts.Count == 0 ? "0 يوم" : string.Join(" و ", parts);
    }
}