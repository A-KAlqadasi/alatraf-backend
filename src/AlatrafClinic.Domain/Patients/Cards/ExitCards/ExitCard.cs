using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards;

namespace AlatrafClinic.Domain.Patients.Cards.ExitCards;

public class ExitCard : AuditableEntity<int>
{
    public string? Note { get; set; }
    public int? PatientId { get; set; }
    public Patient? Patient { get; set; }
    // public Sale? Sales { get; set; }
    public RepairCard? RepairCard { get; set; }

    private ExitCard() { }
    private ExitCard(int? patientId, string note, RepairCard repairCard)
    {
        PatientId = patientId;
        Note = note;
        RepairCard = repairCard;
    }

    public static Result<ExitCard> Create(int? patientId, string note, RepairCard repairCard)
    {
        if (patientId is null || patientId <= 0)
        {
            return ExitCardErrors.PatientIdIsRequired;
        }
        return new ExitCard(patientId, note, repairCard);
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