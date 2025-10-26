using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.TherapyCards.Sessions;

public static class SessionProgramErrors
{
    public static readonly Error DiagnosisProgramIdIsRequired =
        Error.Validation(
            "SessionProgram.DiagnosisProgramIdIsRequired",
            "Diagnosis Program Id is required.");
    public static readonly Error SessionIdIsRequired = Error.Validation(
            "SessionProgram.SessionIdIsRequired",
            "Session Id is required.");
    public static readonly Error DoctorSectionRoomIdIsRequired = Error.Validation(
            "SessionProgram.DoctorSectionRoomIdIsRequired",
            "Doctor Room Id is required.");
    
}