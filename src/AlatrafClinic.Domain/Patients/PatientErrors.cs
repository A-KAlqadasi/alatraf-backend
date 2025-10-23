using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Patients;

public static class PatientErrors
{

    public static readonly Error PersonIdRequired =
        Error.Validation("Patient.PersonIdRequired", "Patient PersonId is required.");
    public static Error PatientTypeInvalid =>
        Error.Validation("Patient.PatientTypeInvalid", "Invalid patient type.");
}