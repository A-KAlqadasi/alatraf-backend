using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.Diagnosises.InjuryReasons;
using AlatrafClinic.Domain.Diagnosises.InjurySides;
using AlatrafClinic.Domain.Diagnosises.InjuryTypes;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Services.Tickets;
using AlatrafClinic.Domain.TherapyCards;

namespace AlatrafClinic.Domain.Diagnosises;

public class Diagnosis : AuditableEntity<int>
{
    public string? DiagnosisText { get; set; }
    public DateTime? InjuryDate { get; set; }
    public int? ReasonId { get; set; }
    public InjuryReason? Reason { get; set; }
    public int? SideId { get; set; }
    public InjurySide? Side { get; set; }

    public int? TypeId { get; set; }
    public InjuryType? Type { get; set; }

    // Links
    public int? TicketId { get; set; }
    public Ticket? Ticket { get; set; }
    public int? PatientId { get; set; }
    public Patient? Patient { get; set; }
    public DiagnosisType? DiagnoType { get; set; }

    private readonly List<DiagnosisProgram> _diagnosisPrograms = new();
    public IReadOnlyCollection<DiagnosisProgram> DiagnosisPrograms => _diagnosisPrograms.AsReadOnly();
    // public ICollection<DiagnosisIndustrialParts> DiagnosisIndustrialParts { get; set; } = new List<DiagnosisIndustrialParts>();
    // public ICollection<Sales> Sales { get; set; } = new List<Sales>();
    // public ICollection<RepairCards> RepairCards { get; set; } = new List<RepairCards>();

    public TherapyCard? TherapyCard { get; set; }

    private Diagnosis()
    {
    }
    public Diagnosis(
        string? diagnosisText,
        DateTime? injuryDate,
        int? reasonId,
        int? sideId,
        int? typeId,
        int? ticketId,
        int? patientId,
        DiagnosisType? diagnosisType)
    {
        DiagnosisText = diagnosisText;
        InjuryDate = injuryDate;
        ReasonId = reasonId;
        SideId = sideId;
        TypeId = typeId;
        TicketId = ticketId;
        PatientId = patientId;
        DiagnoType = diagnosisType;
    }
    public static Result<Diagnosis> Create(
        string? diagnosisText,
        DateTime? injuryDate,
        int? reasonId,
        int? sideId,
        int? typeId,
        int? ticketId,
        int? patientId,
        DiagnosisType? diagnosisType)
    {
        if (string.IsNullOrWhiteSpace(diagnosisText))
        {
            return DiagnosisErrors.DiagnosisTextIsRequired;
        }
        if (injuryDate != null && injuryDate > DateTime.UtcNow)
        {
            return DiagnosisErrors.InvalidInjuryDate;
        }
        if (reasonId == null)
        {
            return DiagnosisErrors.InvalidReasonId;
        }
        if (sideId == null)
        {
            return DiagnosisErrors.InvalidSideId;
        }
        if (typeId == null)
        {
            return DiagnosisErrors.InvalidTypeId;
        }
        if (ticketId == null)
        {
            return DiagnosisErrors.InvalidTicketId;
        }
        if (patientId == null)
        {
            return DiagnosisErrors.InvalidPatientId;
        }

        return new Diagnosis(
            diagnosisText,
            injuryDate,
            reasonId,
            sideId,
            typeId,
            ticketId,
            patientId,
            diagnosisType);
    }
    public Result<Updated> Update(
        string? diagnosisText,
        DateTime? injuryDate,
        int? reasonId,
        int? sideId,
        int? typeId,
        int? ticketId,
        int? patientId,
        DiagnosisType? diagnosisType)
    {
        if (string.IsNullOrWhiteSpace(diagnosisText))
        {
            return DiagnosisErrors.DiagnosisTextIsRequired;
        }
        if (injuryDate is not null && injuryDate > DateTime.UtcNow)
        {
            return DiagnosisErrors.InvalidInjuryDate;
        }
        if (reasonId is null)
        {
            return DiagnosisErrors.InvalidReasonId;
        }
        if (sideId is null)
        {
            return DiagnosisErrors.InvalidSideId;
        }
        if (typeId is null)
        {
            return DiagnosisErrors.InvalidTypeId;
        }
        if (ticketId is null)
        {
            return DiagnosisErrors.InvalidTicketId;
        }
        if (patientId is null)
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
    public Result<Updated> UpdateDiagnosisType(DiagnosisType? diagnosisType)
    {
        if (diagnosisType is null || !Enum.IsDefined(typeof(DiagnosisType), diagnosisType))
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
}