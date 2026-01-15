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

}