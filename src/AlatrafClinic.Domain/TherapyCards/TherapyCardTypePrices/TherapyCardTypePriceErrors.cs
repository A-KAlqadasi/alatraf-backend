using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.TherapyCards.TherapyCardTypePrices;

public static class TherapyCardTypePriceErrors
{
    public static readonly Error InvalidTherapyCardType = Error.Validation("TherapyCardTypePrice.InvalidType", "نوع بطاقة العلاج غير صالح");
    public static readonly Error InvalidPrice = Error.Validation("TherapyCardTypePrice.InvalidPrice", "السعر غير صالح لنوع بطاقة العلاج المحدد");
}