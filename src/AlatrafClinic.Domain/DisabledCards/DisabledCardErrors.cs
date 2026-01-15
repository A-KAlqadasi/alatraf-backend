using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.DisabledCards;

public static class DisabledCardErrors
{
    public static readonly Error CardNumberIsRequired = Error.Validation(
        "DisabledCard.CardNumberIsRequired",
        "رقم البطاقة مطلوب."
    );


    public static readonly Error PatientIdIsRequired = Error.Validation(
        "DisabledCard.PatientIdIsRequired",
        "معرف المريض مطلوب"
    );
    public static readonly Error CardNumberDuplicated = Error.Conflict("DisabledCard.CardNumberDuplicate", "رقم البطاقة موجود بالفعل!");
    public static readonly Error DisabledCardNotFound = Error.NotFound("DisabledCard.DisabledCardNotFound", "البطاقة غير موجودة!");
    public static readonly Error IssueDateInvalid = Error.Validation(
        "DisabledCard.IssueDateInvalid",
        "تاريخ الإصدار لا يمكن أن يكون في المستقبل"
    );
    public static readonly Error DisabilityTypeIsRequired = Error.Validation(
        "DisabledCard.DisabilityTypeIsRequired",
        "نوع الإعاقة مطلوب"
    );
    public static readonly Error DisabledCardDesnotBelongToPatient = Error.Conflict(
        "DisabledCard.DisabledCardDesnotBelongToPatient",
        "البطاقة لا تنتمي إلى المريض"
    );
}