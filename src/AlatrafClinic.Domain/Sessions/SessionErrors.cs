using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Sessions;

public static class SessionErrors
{
    public static readonly Error TherapyCardIdIsRequired =
        Error.Validation(
            "Session.TherapyCardIdIsRequired",
            "معرفه بطاقة العلاج مطلوب.");
    public static readonly Error NumberIsRequired =
        Error.Validation(
            "Session.NumberIsRequired",
            "رقم الجلسة مطلوب.");
    public static readonly Error SessionAlreadyTaken =
        Error.Validation(
            "Session.SessionAlreadyTaken",
            "هذه الجلسة مأخوذة بالفعل.");
    public static Error InvalidSessionDate(DateOnly sessionDate) =>
        Error.Validation(
            "Session.InvalidSessionDate",
            $"يجب أن يكون تاريخ الجلسة في {sessionDate.ToString("dd/MM/yyyy")}.");
    public static readonly Error SessionProgramsAreRequired =
        Error.Validation(
            "Session.SessionProgramsAreRequired",
            "مطلوب برنامج جلسة واحد على الأقل.");
    public static readonly Error SessionNotFound =
        Error.NotFound(
            "Session.SessionNotFound",
            "لم يتم العثور على الجلسة المحددة.");
}