using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Sessions;

public static class SessionProgramErrors
{
    public static readonly Error DiagnosisProgramIdIsRequired =
        Error.Validation(
            "SessionProgram.DiagnosisProgramIdIsRequired",
            "معرف برنامج التشخيص مطلوب.");
    public static readonly Error DoctorSectionRoomIdIsRequired = Error.Validation(
            "SessionProgram.DoctorSectionRoomIdIsRequired",
            "معرف غرفة قسم الطبيب مطلوب.");
    
}