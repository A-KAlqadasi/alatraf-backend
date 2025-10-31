using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.RepairCards.AttendanceTimes;

public class AttendanceTime : AuditableEntity<int>
{
    public DateTime AttendanceDate { get; private set; }
    public string? Note { get; private set; }
    public int RepairCardId { get; private set; }
    public RepairCard? RepairCard { get; set; }

    private AttendanceTime() { }
    private AttendanceTime(int repairCardId, DateTime attendanceDate, string? note)
    {
        RepairCardId = repairCardId;
        AttendanceDate = attendanceDate;
        Note = note;
    }
    public static Result<AttendanceTime> Create(int repairCardId, DateTime attendanceDate, string? note)
    {
        if (repairCardId <= 0)
        {
            return AttendanceTimeErrors.RepairCardIsRequired;
        }
        if (attendanceDate.Date < DateTime.Now.Date)
        {
            return AttendanceTimeErrors.AttendanceTimeInPast;
        }

        return new AttendanceTime(repairCardId, attendanceDate, note);
    }
}