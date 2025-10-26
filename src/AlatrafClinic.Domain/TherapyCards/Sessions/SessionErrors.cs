using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.TherapyCards.Sessions;

public static class SessionErrors
{
    public static readonly Error TherapyCardIdIsRequired =
        Error.Validation(
            "Session.TherapyCardIdIsRequired",
            "Therapy Card Id is required.");
    public static readonly Error NumberIsRequired =
        Error.Validation(
            "Session.NumberIsRequired",
            "Session number is required.");
    public static readonly Error SessionAlreadyTaken =
        Error.Validation(
            "Session.SessionAlreadyTaken",
            "This session is already taken.");
    public static Error InvalidSessionDate(DateTime sessionDate) =>
        Error.Validation(
            "Session.InvalidSessionDate",
            $"Session date must be in {sessionDate.ToString("dd/MM/yyyy")}.");
}