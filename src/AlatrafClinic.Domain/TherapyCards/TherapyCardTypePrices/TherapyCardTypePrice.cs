using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards.Enums;

namespace AlatrafClinic.Domain.TherapyCards.TherapyCardTypePrices;

public class TherapyCardTypePrice : AuditableEntity<int>
{
    public TherapyCardType Type { get; private set; }
    public decimal SessionPrice { get; private set; }

    private TherapyCardTypePrice() { }

    private TherapyCardTypePrice(TherapyCardType type, decimal sessionPrice)
    {
        Type = type;
        SessionPrice = sessionPrice;
    }
    public static Result<TherapyCardTypePrice> Create(TherapyCardType type, decimal sessionPrice)
    {
        if (!Enum.IsDefined(typeof(TherapyCardType), type))
        {
            return TherapyCardTypePriceErrors.InvalidTherapyCardType;
        }
        if (sessionPrice <= 0)
        {
            return TherapyCardTypePriceErrors.InvalidPrice;
        }

        return new TherapyCardTypePrice(type, sessionPrice);
    }

    public Result<Updated> UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
        {
            return TherapyCardTypePriceErrors.InvalidPrice;
        }

        SessionPrice = newPrice;
        return Result.Updated;
    }
}