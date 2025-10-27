using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.RepairCards.AttendanceTimes;

public class AttendanceTime : AuditableEntity<int>
{
    public DateTime AttendanceDate { get; set; }
    public string? Note { get; set; }
    public int? RepairCardId { get; set; }
    public RepairCard? RepairCard { get; set; }

    private AttendanceTime() { }
    private AttendanceTime(DateTime attendanceDate, string? note)
    {
        AttendanceDate = attendanceDate;
        Note = note;
    }
    public static Result<AttendanceTime> Create(DateTime attendanceDate, string? note)
    {
        if (attendanceDate.Date < DateTime.Now.Date)
        {
            return AttendanceTimeErrors.AttendanceTimeInPast;
        }

        return new AttendanceTime(attendanceDate, note);
    }
}