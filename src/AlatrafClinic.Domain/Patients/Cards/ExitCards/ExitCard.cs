using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Patients.Cards.ExitCards;

public class ExitCard : AuditableEntity<int>
{
    public string? Note { get; set; }
    public int? PatientId { get; set; }
    public Patient? Patient { get; set; }
    // public Sale? Sales { get; set; }
    // public RepairCard? RepairCards { get; set; }

    private ExitCard() { }
    public ExitCard(int? patientId, string note)
    {
        PatientId = patientId;
        Note = note;
    }
    public static Result<ExitCard> Create(int? patientId, string note)
    {
        if (patientId is null || patientId <= 0)
        {
            return ExitCardErrors.PatientIdIsRequired;
        }
        return new ExitCard(patientId, note);
    }
    
    public Result<Updated> Update(int? patientId, string note)
    {
        if (patientId is null || patientId <= 0)
        {
            return ExitCardErrors.PatientIdIsRequired;
        }

        PatientId = patientId;
        Note = note;
        return Result.Updated;
    }
}