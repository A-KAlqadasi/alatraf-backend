using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.RepairCards.AttendanceTimes;

public static class AttendanceTimeErrors
{
    public static readonly Error AttendanceTimeInPast = Error.Validation("AttendanceTime.InPast", "Attendance time must be in future");
}