using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Diagnosises.InjuryTypes;

public static class InjuryTypeErrors
{
    public static readonly Error NameIsRequired = Error.Validation(
        "InjuryType.NameIsRequired",
        "اسم نوع الإصابة مطلوب");
}