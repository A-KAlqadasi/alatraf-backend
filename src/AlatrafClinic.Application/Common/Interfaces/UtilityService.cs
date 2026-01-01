using System.Globalization;

namespace AlatrafClinic.Application.Common.Interfaces;

public static class UtilityService
{
    public static int CalculateAge(DateOnly dateOfBirth, DateOnly now)
    {
        var age = now.Year - dateOfBirth.Year;

        if (dateOfBirth > now.AddYears(-age))
        {
            age -= 1;
        }

        return age;
    }
    
    public static string GenderToArabicString(bool gender)
    {
        return gender ? "ذكر" : "أنثى";
    }
    public static string GetDayNameArabic(DateOnly date)
    {
        // "dddd" format specifier gives the full day name
        return date.ToString("dddd", new CultureInfo("ar-SA")); 
    }
}