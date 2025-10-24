using System.Security.Cryptography.X509Certificates;

using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;
using AlatrafClinic.Domain.Organization.DoctorSectionRooms;

namespace AlatrafClinic.Domain.TherapyCards.Sessions;

public class SessionProgram : AuditableEntity<int>
{
    public int? DiagnosisProgramId { get; set; }
    public DiagnosisProgram? DiagnosisProgram { get; set; }
    public int? SessionId { get; set; }
    public Session? Session { get; set; }
    public int? DoctorSectionRoomId { get; set; }
    public DoctorSectionRoom? DoctorSectionRoom { get; set; }
    private SessionProgram()
    {
    }
    private SessionProgram(int diagnosisProgramId, int sessionId, int doctorSectionRoomId)
    {
        DiagnosisProgramId = diagnosisProgramId;
        SessionId = sessionId;
        DoctorSectionRoomId = doctorSectionRoomId;
    }
    public static Result<SessionProgram> Create(int diagnosisProgramId, int sessionId, int doctorSectionRoomId)
    {
        if (diagnosisProgramId <= 0)
        {
            return SessionProgramErrors.DiagnosisProgramIdIsRequired;
        }
        if (sessionId <= 0)
        {
            return SessionProgramErrors.SessionIdIsRequired;
        }
        if (doctorSectionRoomId <= 0)
        {
            return SessionProgramErrors.DoctorSectionRoomIdIsRequired;
        }

        return new SessionProgram(diagnosisProgramId, sessionId, doctorSectionRoomId);
    }
    public Result<Updated> UpdateDoctorSectionRoom(int doctorSectionRoomId)
    {
        if (doctorSectionRoomId <= 0)
        {
            return SessionProgramErrors.DoctorSectionRoomIdIsRequired;
        }
        DoctorSectionRoomId = doctorSectionRoomId;
        return Result.Updated;
    }
}