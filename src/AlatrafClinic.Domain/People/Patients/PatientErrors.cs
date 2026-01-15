using System.Security.Cryptography;

using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Patients;

public static class PatientErrors
{
    public static readonly Error PersonIdRequired =
        Error.Validation("Patient.PersonIdRequired", "معرفه الشخص مطلوب");
    public static Error PatientTypeInvalid =>
        Error.Validation("Patient.PatientTypeInvalid", "نوع المريض غير صالح");
    public static readonly Error PatientNotFound =
        Error.NotFound("Patient.NotFound", "المريض غير موجود");
}