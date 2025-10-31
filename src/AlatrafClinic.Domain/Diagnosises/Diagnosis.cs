using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;
using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.Diagnosises.InjuryReasons;
using AlatrafClinic.Domain.Diagnosises.InjurySides;
using AlatrafClinic.Domain.Diagnosises.InjuryTypes;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.RepairCards;
using AlatrafClinic.Domain.Sales;
using AlatrafClinic.Domain.Services.Tickets;
using AlatrafClinic.Domain.TherapyCards;

namespace AlatrafClinic.Domain.Diagnosises;

public class Diagnosis : AuditableEntity<int>
{
    public string DiagnosisText { get; private set; } = string.Empty;
    public DateTime InjuryDate { get; private set; }
    public int ReasonId { get; private set; }
    public InjuryReason? Reason { get; set; }
    public int SideId { get; private set; }
    public InjurySide? Side { get; set; }

    public int TypeId { get; private set; }
    public InjuryType? Type { get; set; }

    // Links
    public int TicketId { get; private set; }
    public Ticket? Ticket { get; set; }
    public int PatientId { get; private set; }
    public Patient? Patient { get; set; }
    public DiagnosisType DiagnoType { get; private set; }

    private readonly List<DiagnosisProgram> _diagnosisPrograms = new();
    public IReadOnlyCollection<DiagnosisProgram> DiagnosisPrograms => _diagnosisPrograms.AsReadOnly();
    private readonly List<DiagnosisIndustrialPart> _diagnosisIndustrialParts = new();
    public IReadOnlyCollection<DiagnosisIndustrialPart> DiagnosisIndustrialParts => _diagnosisIndustrialParts.AsReadOnly();
    public TherapyCard? TherapyCard { get; set; }
    public RepairCard? RepairCard { get; set; }
    public Sale? Sale { get; set; }

    private Diagnosis()
    {
    }
    public Diagnosis(
        int ticketId,
        string diagnosisText,
        DateTime injuryDate,
        int reasonId,
        int sideId,
        int typeId,
        int patientId,
        DiagnosisType diagnosisType)
    {
        TicketId = ticketId;
        DiagnosisText = diagnosisText;
        InjuryDate = injuryDate;
        ReasonId = reasonId;
        SideId = sideId;
        TypeId = typeId;
        PatientId = patientId;
        DiagnoType = diagnosisType;
    }
    public static Result<Diagnosis> Create(
        int ticketId,
        string diagnosisText,
        DateTime injuryDate,
        int reasonId,
        int sideId,
        int typeId,
        int patientId,
        DiagnosisType diagnosisType)
    {
        if(ticketId <= 0)
        {
            return DiagnosisErrors.InvalidTicketId;
        }
        if (string.IsNullOrWhiteSpace(diagnosisText))
        {
            return DiagnosisErrors.DiagnosisTextIsRequired;
        }
        if (injuryDate > DateTime.UtcNow)
        {
            return DiagnosisErrors.InvalidInjuryDate;
        }
        if (reasonId <= 0)
        {
            return DiagnosisErrors.InvalidReasonId;
        }
        if (sideId <= 0)
        {
            return DiagnosisErrors.InvalidSideId;
        }
        if (typeId <= 0)
        {
            return DiagnosisErrors.InvalidTypeId;
        }
        
        if (patientId <= 0)
        {
            return DiagnosisErrors.InvalidPatientId;
        }

        return new Diagnosis(
            ticketId,
            diagnosisText,
            injuryDate,
            reasonId,
            sideId,
            typeId,
            patientId,
            diagnosisType);
    }
    public Result<Updated> Update(
        string diagnosisText,
        DateTime injuryDate,
        int reasonId,
        int sideId,
        int typeId,
        int ticketId,
        int patientId,
        DiagnosisType diagnosisType)
    {
        if (string.IsNullOrWhiteSpace(diagnosisText))
        {
            return DiagnosisErrors.DiagnosisTextIsRequired;
        }
        if (injuryDate > DateTime.UtcNow)
        {
            return DiagnosisErrors.InvalidInjuryDate;
        }
        if (reasonId <= 0)
        {
            return DiagnosisErrors.InvalidReasonId;
        }
        if (sideId <= 0)
        {
            return DiagnosisErrors.InvalidSideId;
        }
        if (typeId <= 0)
        {
            return DiagnosisErrors.InvalidTypeId;
        }
        if (ticketId <= 0)
        {
            return DiagnosisErrors.InvalidTicketId;
        }
        if (patientId <= 0)
        {
            return DiagnosisErrors.InvalidPatientId;
        }

        DiagnosisText = diagnosisText;
        InjuryDate = injuryDate;
        ReasonId = reasonId;
        SideId = sideId;
        TypeId = typeId;
        TicketId = ticketId;
        PatientId = patientId;
        DiagnoType = diagnosisType;

        return Result.Updated;
    }
    public Result<Updated> UpdateDiagnosisType(DiagnosisType diagnosisType)
    {
        if (!Enum.IsDefined(typeof(DiagnosisType), diagnosisType))
        {
            return DiagnosisErrors.InvalidDiagnosisType;
        }

        DiagnoType = diagnosisType;
        return Result.Updated;
    }

    public Result<Updated> AssignDiagnosisPrograms(List<DiagnosisProgram> diagnosisPrograms)
    {
        if (DiagnoType != DiagnosisType.Therapy)
        {
            return DiagnosisErrors.DiagnosisProgramAdditionOnlyForTherapyDiagnosis;
        }
        if (diagnosisPrograms.Count() <= 0)
        {
            return DiagnosisErrors.MedicalProgramsAreRequired;
        }

        _diagnosisPrograms.AddRange(diagnosisPrograms);
        
        return Result.Updated;
    }

    public Result<Updated> AssignTherapyCard(TherapyCard therapyCard)
    {
        if (DiagnoType != DiagnosisType.Therapy)
        {
            return DiagnosisErrors.TherapyCardAdditionOnlyForTherapyDiagnosis;
        }
        if (TherapyCard != null)
        {
            return DiagnosisErrors.TherapyCardAlreadyAssigned;
        }
        TherapyCard = therapyCard;
        return Result.Updated;
    }

    public Result<Updated> AssignRepairCard(RepairCard repairCard)
    {
        if (DiagnoType != DiagnosisType.Limbs)
        {
            return DiagnosisErrors.RepairCardAdditionOnlyForLimbsDiagnosis;
        }
        if (RepairCard != null)
        {
            return DiagnosisErrors.RepairCardAlreadyAssigned;
        }
        
        RepairCard = repairCard;
        return Result.Updated;
    }

    public Result<Updated> AssignDiagnosisIndustrialParts(List<DiagnosisIndustrialPart> diagnosisIndustrialParts)
    {
        if (DiagnoType != DiagnosisType.Limbs)
        {
            return DiagnosisErrors.IndustrialPartAdditionOnlyForLimbsDiagnosis;
        }
        if (diagnosisIndustrialParts.Count() <= 0)
        {
            return DiagnosisErrors.IndustrialPartsAreRequired;
        }

        _diagnosisIndustrialParts.AddRange(diagnosisIndustrialParts);

        return Result.Updated;
    }

    public Result<Updated> AssignSale(Sale sale)
    {
        if (Sale != null)
        {
            return DiagnosisErrors.SaleAlreadyAssigned;
        }
        if (DiagnoType != DiagnosisType.Sales)
        {
            return DiagnosisErrors.SaleAssignmentOnlyForSalesDiagnosis;
        }

        Sale = sale;
        return Result.Updated;
    }
}