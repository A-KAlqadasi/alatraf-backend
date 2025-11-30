using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.RepairCards;
using AlatrafClinic.Domain.Sales;

namespace AlatrafClinic.Domain.ExitCards;

public class ExitCard : AuditableEntity<int>
{
    public string? Note { get; private set; }
    public int PatientId { get; private set; }
    public Patient? Patient { get; private set; }
    public Sale? Sale { get; set; }
    public int? SaleId { get; private set; }
    public RepairCard? RepairCard { get; set; }
    public int? RepairCardId { get; set; }

    private ExitCard() { }
    private ExitCard(int patientId, string? note)
    {
        PatientId = patientId;
        Note = note;
    }

    public static Result<ExitCard> Create(int patientId, string? note)
    {
        if (patientId <= 0)
        {
            return ExitCardErrors.PatientIdIsRequired;
        }
        return new ExitCard(patientId, note);
    }

    public Result<Updated> Update(int patientId, string? note)
    {
        if (patientId <= 0)
        {
            return ExitCardErrors.PatientIdIsRequired;
        }

        PatientId = patientId;
        Note = note;
        return Result.Updated;
    }

    public Result<Updated> AssignRepairCard(RepairCard repairCard)
    {
        if (repairCard is null)
        {
            return ExitCardErrors.RepairCardIsRequired;
        }

        if (Sale is not null)
        {
            return ExitCardErrors.AlreadyAssignedToSale;
        }

        RepairCard = repairCard;
        RepairCardId = repairCard.Id;
        return Result.Updated;
    }
    public Result<Updated> AssignSale(Sale sale)
    {
        
        if (sale is null)
        {
            return ExitCardErrors.SaleIsRequired;
        }

        if (RepairCard is not null)
        {
            return ExitCardErrors.AlreadyAssignedToRepairCard;
        }

        Sale = sale;
        SaleId = sale.Id;
        return Result.Updated;
    }
}