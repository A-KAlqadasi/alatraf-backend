using System.Security.Cryptography;

using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Patients;

public static class PatientErrors
{
    public static readonly Error PersonIdRequired =
        Error.Validation("Patient.PersonIdRequired", "Patient PersonId is required.");
    public static Error PatientTypeInvalid =>
        Error.Validation("Patient.PatientTypeInvalid", "Invalid patient type.");
    public static readonly Error PatientNotFound =
        Error.NotFound("Patient.NotFound", "Patient not found.");
}