namespace AlatrafClinic.Application.Common.Interfaces;

public static class UtilityService
{
    public static int CalculateAge(DateOnly birthdate, DateOnly today)
    {
        var age = today.Year - birthdate.Year;
        if (birthdate > today.AddYears(-age)) age--;
        return age;
    }
}