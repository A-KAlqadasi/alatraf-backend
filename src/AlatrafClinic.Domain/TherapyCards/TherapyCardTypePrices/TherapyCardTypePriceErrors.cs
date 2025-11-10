using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.TherapyCards.TherapyCardTypePrices;

public static class TherapyCardTypePriceErrors
{
    public static readonly Error InvalidTherapyCardType = Error.Validation("TherapyCardTypePrice.InvalidType", "Therapy card type is invalid");
    public static readonly Error InvalidPrice = Error.Validation("TherapyCardTypePrice.InvalidPrice", "Price invalid for therapy card type");
}