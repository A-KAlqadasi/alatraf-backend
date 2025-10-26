using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Enums;
using AlatrafClinic.Domain.People;

namespace AlatrafClinic.Domain.Patients;

public class Patient : AuditableEntity<int>
{
    public int PersonId { get; set; }
    public Person? Person { get; set; }
    public PatientType? PatientType { get; set; }
    public string? AutoRegistrationNumber { get; set; }

    private Patient() { }

    private Patient(int personId, PatientType? patientType)
    {
        PersonId = personId;
        PatientType = patientType;
    }

    public static Result<Patient> Create(int personId, PatientType patientType)
    {
        if (personId <= 0)
        {
            return PatientErrors.PersonIdRequired;
        }

        if (!Enum.IsDefined(typeof(PatientType), patientType))
        {
            return PatientErrors.PatientTypeInvalid;
        }
        return new Patient(personId, patientType);
    }
    
    public Result<Updated> Update(int personId, PatientType patientType)
    {
        if (personId <= 0)
        {
            return PatientErrors.PersonIdRequired;
        }

        if (!Enum.IsDefined(typeof(PatientType), patientType))
        {
            return PatientErrors.PatientTypeInvalid;
        }
        PersonId = personId;
        PatientType = patientType;

        return Result.Updated;
    }
}