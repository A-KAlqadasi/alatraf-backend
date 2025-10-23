using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Diagnosises.InjurySides;

public static class InjurySideErrors
{
    public static readonly Error NameIsRequired = Error.Validation(
        "InjurySide.NameIsRequired",
        "Injury side name is required.");
}