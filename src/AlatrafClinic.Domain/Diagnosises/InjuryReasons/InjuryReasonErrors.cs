using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Diagnosises.InjuryReasons;

public static class InjuryReasonErrors
{
    public static readonly Error NameIsRequired = Error.Validation(
        "InjuryReason.NameIsRequired",
        "اسم سبب الإصابة مطلوب");
}