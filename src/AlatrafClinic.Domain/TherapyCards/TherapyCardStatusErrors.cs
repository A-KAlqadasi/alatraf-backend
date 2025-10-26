using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.TherapyCards;

public static class TherapyCardStatusErrors
{
    public static readonly Error InvalidStatus =
    Error.Validation("TherapyCardStatus.InvalidStatus", "Therapy Card Status is invalid");
    public static readonly Error InvalidPaymentId =
    Error.Validation("TherapyCardStatus.InvalidPaymentId", "Payment Id is invalid");
    public static readonly Error NewStatusExists = Error.Conflict("TherapyCardStatus.NewStatusTwice", "Cannot add 'New' status twice for the same card.");
    public static readonly Error CardNotExpiredToRenew = Error.Conflict("TherapyCardStatus.CardNotExpiredToRenew", "Therapy Card is not expired to renew.");
    public static readonly Error CardExpiredToReplace = Error.Conflict("TherapyCardStatus.CardExpiredToReplace", "Therapy Card is expired to replace.");

}
