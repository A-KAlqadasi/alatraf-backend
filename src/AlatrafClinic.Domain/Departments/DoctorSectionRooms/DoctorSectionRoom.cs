using System.Net.Http.Headers;

using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;
using AlatrafClinic.Domain.Organization.Doctors;
using AlatrafClinic.Domain.Organization.Rooms;
using AlatrafClinic.Domain.Organization.Sections;
using AlatrafClinic.Domain.TherapyCards.Sessions;

namespace AlatrafClinic.Domain.Organization.DoctorSectionRooms;

public class DoctorSectionRoom : AuditableEntity<int>
{
    public int DoctorId { get; private set; }
    public Doctor Doctor { get; private set; } = default!;

    public int SectionId { get; private set; }
    public Section Section { get; private set; } = default!;

    public int? RoomId { get; private set; }     // optional now
    public Room? Room { get; private set; }

    public DateTime AssignDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsActive { get; private set; }
    public string? Notes { get; private set; }

    public ICollection<DiagnosisIndustrialPart> DiagnosisIndustrialParts { get; private set; } = new List<DiagnosisIndustrialPart>();
    public ICollection<SessionProgram> SessionPrograms { get; private set; } = new List<SessionProgram>();

    private DoctorSectionRoom() { }

    private DoctorSectionRoom(int doctorId, int sectionId, int? roomId, string? notes)
    {
        DoctorId = doctorId;
        SectionId = sectionId;
        RoomId = roomId;
        AssignDate = DateTime.UtcNow;
        IsActive = true;
        Notes = notes;
    }

    public static Result<DoctorSectionRoom> AssignToSection(int doctorId, int sectionId, string? notes = null)
    {
        if (sectionId <= 0)
        {
            return DoctorSectionRoomErrors.SectionIdRequired;
        }
        if (doctorId <= 0)
        {
            return DoctorSectionRoomErrors.DoctorIdRequired;
        }

        return new DoctorSectionRoom(doctorId, sectionId, null, notes);
    }

    public static Result<DoctorSectionRoom> AssignToRoom(int doctorId, int sectionId, int roomId, string? notes = null)
    {
        if (roomId <= 0)
        {
            return DoctorSectionRoomErrors.RoomIdRequired;
        }
        if (sectionId <= 0)
        {
            return DoctorSectionRoomErrors.SectionIdRequired;
        }
        if (doctorId <= 0)
        {
            return DoctorSectionRoomErrors.DoctorIdRequired;
        }

        return new DoctorSectionRoom(doctorId, sectionId, roomId, notes);
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