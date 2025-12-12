
using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections;
using AlatrafClinic.Domain.Departments.Sections.Rooms;
using AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;
using AlatrafClinic.Domain.People.Doctors;
using AlatrafClinic.Domain.TherapyCards.Sessions;

namespace AlatrafClinic.Domain.Departments.DoctorSectionRooms;

public class DoctorSectionRoom : AuditableEntity<int>
{
    public int DoctorId { get; private set; }
    public Doctor Doctor { get; private set; } = default!;

    public int SectionId { get; private set; }
    public Section Section { get; private set; } = default!;

    public int? RoomId { get; private set; }     // optional now
    public Room? Room { get; private set; }

    public DateOnly AssignDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
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
        AssignDate = AlatrafClinicConstants.TodayDate;
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
        EndDate = AlatrafClinicConstants.TodayDate;
        return Result.Updated;
    }

    public int GetTodayIndustrialPartsCount()
    {
        var today = AlatrafClinicConstants.TodayDate;
        return DiagnosisIndustrialParts.Count(dip => DateOnly.FromDateTime(dip.CreatedAtUtc.Date) == today);
    }
    public int GetTodaySessionsCount()
    {
        var today = AlatrafClinicConstants.TodayDate;
        return SessionPrograms.Count(sp => DateOnly.FromDateTime(sp.CreatedAtUtc.Date) == today);
    }
}