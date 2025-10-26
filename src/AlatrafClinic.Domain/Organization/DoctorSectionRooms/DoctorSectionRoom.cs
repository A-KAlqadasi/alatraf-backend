using System.Net.Http.Headers;

using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.Doctors;
using AlatrafClinic.Domain.Organization.Rooms;
using AlatrafClinic.Domain.Organization.Sections;

namespace AlatrafClinic.Domain.Organization.DoctorSectionRooms;

public class DoctorSectionRoom : AuditableEntity<int>
{
   public int DoctorId { get; private set; }
    public Doctor Doctor { get; private set; } = default!;
    public int RoomId { get; private set; }
    public Room Room { get; private set; } = default!;
    public DateTime AssignDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsActive { get; private set; }
    public string? Notes { get; private set; }

    private DoctorSectionRoom() { }

    private DoctorSectionRoom(int doctorId, int roomId, DateTime assignDate, string? notes)
    {
        DoctorId = doctorId;
        RoomId = roomId;
        AssignDate = assignDate;
        IsActive = true;
        Notes = notes;
    }

    public static Result<DoctorSectionRoom> Assign(int doctorId, int roomId, string? notes = null)
    {
        return new DoctorSectionRoom(doctorId, roomId, DateTime.UtcNow, notes);
    }

    public Result<Updated> EndAssignment()
    {
        if (!IsActive)
            return DoctorSectionRoomErrors.AssignmentAlreadyEnded;

        IsActive = false;
        EndDate = DateTime.UtcNow;
        return Result.Updated;
    }
}