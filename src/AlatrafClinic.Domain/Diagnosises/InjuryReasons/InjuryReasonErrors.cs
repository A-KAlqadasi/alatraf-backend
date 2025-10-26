using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Diagnosises.InjuryReasons;

public static class InjuryReasonErrors
{
    public static readonly Error NameIsRequired = Error.Validation(
        "InjuryReason.NameIsRequired",
        "Injury reason name is required.");
}