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
}