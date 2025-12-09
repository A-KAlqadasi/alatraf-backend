using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.DisabledCards;
using AlatrafClinic.Domain.ExitCards;
using AlatrafClinic.Domain.Patients.Enums;
using AlatrafClinic.Domain.People;
using AlatrafClinic.Domain.Services.Tickets;
using AlatrafClinic.Domain.WoundedCards;

namespace AlatrafClinic.Domain.Patients;

public class Patient : AuditableEntity<int>
{
    public int PersonId { get; private set; }
    public Person Person { get; set; } = default!;
    public PatientType PatientType { get; private set; }

    // navigations
    public ICollection<Ticket> Tickets = new List<Ticket>();
    public ICollection<Diagnosis> Diagnoses = new List<Diagnosis>();
    public ICollection<ExitCard> ExitCards = new List<ExitCard>();
    public WoundedCard? WoundedCard { get; private set; }
    public DisabledCard? DisabledCard { get; private set; }

    private Patient() { }

    private Patient(int personId, PatientType patientType)
    {
        PersonId = personId;
        PatientType = patientType;
    }

    public static Result<Patient> Create(int personId, PatientType patientType)
    {
        // if (personId <= 0)
        // {
        //     return PatientErrors.PersonIdRequired;
        // }

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