using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards.Enums;

namespace AlatrafClinic.Domain.TherapyCards;

public class TherapyCardStatus : AuditableEntity<int>
{
    public int? TherapyCardId { get; set; }
    public TherapyCard? TherapyCard { get; set; }

    public CardStatus? Status { get; set; }

    public int? PaymentId { get; set; }
   //public Payment? Payment { get; set; }
    private TherapyCardStatus()
    {

    }
    private TherapyCardStatus(CardStatus status)
    {
        Status = status;
    }

    public static Result<TherapyCardStatus> Create(CardStatus status)
    {
        if (!Enum.IsDefined(typeof(CardStatus), status))
        {
            return TherapyCardStatusErrors.InvalidStatus;
        }
        return new TherapyCardStatus(status);
    }

}