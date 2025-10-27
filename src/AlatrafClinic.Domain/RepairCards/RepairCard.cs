using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Patients.Cards.ExitCards;
using AlatrafClinic.Domain.RepairCards.AttendanceTimes;
using AlatrafClinic.Domain.RepairCards.Enums;

namespace AlatrafClinic.Domain.RepairCards;

public class RepairCard : AuditableEntity<int>
{
    public RepairCardStatus Status { get; set; }
    public bool? IsActive { get; set; }
    public int DiagnosisId { get; set; }
    public Diagnosis? Diagnosis { get; set; }
    public int? PaymentId { get; set; }
    //public Payment? Payment { get; set; }
    public int? ExitCardId { get; set; }
    public ExitCard? ExitCard { get; set; }

    // Navigation
    public AttendanceTime? AttendanceTime { get; set; }

    //public ICollection<Order> Orders { get; set; } = new List<Order>();
    private readonly List<DiagnosisIndustrialPart> _diagnosisIndustrialParts = new();
    public IReadOnlyCollection<DiagnosisIndustrialPart> DiagnosisIndustrialParts => _diagnosisIndustrialParts.AsReadOnly();

    private RepairCard() { }

    private RepairCard(RepairCardStatus status, List<DiagnosisIndustrialPart> industrialParts, bool isActive = true)
    {
        IsActive = isActive;
        Status = status;
        _diagnosisIndustrialParts = industrialParts;
    }

    public static Result<RepairCard> Create(List<DiagnosisIndustrialPart> diagnosisIndustrialParts)
    {
        if (diagnosisIndustrialParts == null || !diagnosisIndustrialParts.Any())
        {
            return RepairCardErrors.DiagnosisIndustrialPartsAreRequired;
        }

        return new RepairCard(RepairCardStatus.New, diagnosisIndustrialParts);
    }

    public bool IsEditable => Status is not RepairCardStatus.LegalExit or RepairCardStatus.IllegalExit;
    public bool CanTransitionTo(RepairCardStatus newStatus)
    {
        return (Status, newStatus) switch
        {
            (RepairCardStatus.New, RepairCardStatus.AssignedToTechnician) => true,
            (RepairCardStatus.AssignedToTechnician, RepairCardStatus.AssignedToTechnician) => true,
            (RepairCardStatus.AssignedToTechnician, RepairCardStatus.InProgress) => true,
            (RepairCardStatus.InProgress, RepairCardStatus.Completed) => true,
            (RepairCardStatus.Completed, RepairCardStatus.InTraining) => true,
            (RepairCardStatus.Completed, RepairCardStatus.ExitForPractice) => true,
            (RepairCardStatus.Completed, RepairCardStatus.LegalExit) => true,
            (RepairCardStatus.InTraining, RepairCardStatus.ExitForPractice) => true,
            (RepairCardStatus.ExitForPractice, RepairCardStatus.LegalExit) => true,
            (_, RepairCardStatus.IllegalExit) when Status != RepairCardStatus.LegalExit => true,
            _ => false
        };
    }
    public Result<Updated> AssignRepairCardToDoctor(int doctorSectionRoomId)
    {
        if (!IsEditable)
        {
            return RepairCardErrors.Readonly;
        }
        if (!CanTransitionTo(RepairCardStatus.AssignedToTechnician))
        {
            return RepairCardErrors.InvalidStateTransition(Status, RepairCardStatus.AssignedToTechnician);
        }
        
        _diagnosisIndustrialParts.ForEach(i => i.AssignDoctor(doctorSectionRoomId));
        Status = RepairCardStatus.AssignedToTechnician;
        return Result.Updated;
    }
    public Result<Updated> AssignSpecificIndustrialPartToDoctor(int diagnosisIndustrialPartId, int doctorSectionRoomId)
    {
        if (!IsEditable)
        {
            return RepairCardErrors.Readonly;
        }
        if (!CanTransitionTo(RepairCardStatus.AssignedToTechnician))
        {
            return RepairCardErrors.InvalidStateTransition(Status, RepairCardStatus.AssignedToTechnician);
        }

        var industrialPart = _diagnosisIndustrialParts.FirstOrDefault(i => i.Id == diagnosisIndustrialPartId);

        if (industrialPart is null)
        {
            return RepairCardErrors.DiagnosisIndustrialPartNotFound;
        }

        Status = RepairCardStatus.AssignedToTechnician;
        industrialPart.AssignDoctor(doctorSectionRoomId);
        return Result.Updated;
    }
    public Result<Updated> MarkAsInProgress()
    {
        if (!IsEditable)
        {
            return RepairCardErrors.Readonly;
        }
        if (!CanTransitionTo(RepairCardStatus.InProgress))
        {
            return RepairCardErrors.InvalidStateTransition(Status, RepairCardStatus.InProgress);
        }
        Status = RepairCardStatus.InProgress;
        return Result.Updated;
    }
    public Result<Updated> MarkAsCompleted()
    {
        if (!IsEditable)
        {
            return RepairCardErrors.Readonly;
        }
        if (!CanTransitionTo(RepairCardStatus.Completed))
        {
            return RepairCardErrors.InvalidStateTransition(Status, RepairCardStatus.Completed);
        }
        Status = RepairCardStatus.Completed;
        return Result.Updated;
    }
    public Result<Updated> MarkAsInTraining()
    {
        if (!IsEditable)
        {
            return RepairCardErrors.Readonly;
        }
        if (!CanTransitionTo(RepairCardStatus.InTraining))
        {
            return RepairCardErrors.InvalidStateTransition(Status, RepairCardStatus.InTraining);
        }
        Status = RepairCardStatus.InTraining;
        return Result.Updated;
    }
    public Result<Updated> MarkAsIllegalExit()
    {
        if (!IsEditable)
        {
            return RepairCardErrors.Readonly;
        }
        if (!CanTransitionTo(RepairCardStatus.IllegalExit))
        {
            return RepairCardErrors.InvalidStateTransition(Status, RepairCardStatus.IllegalExit);
        }
        Status = RepairCardStatus.IllegalExit;
        return Result.Updated;
    }
    public Result<Updated> MarkAsLegalExit()
    {
        if (!IsEditable)
        {
            return RepairCardErrors.Readonly;
        }
        if (!CanTransitionTo(RepairCardStatus.LegalExit))
        {
            return RepairCardErrors.InvalidStateTransition(Status, RepairCardStatus.LegalExit);
        }
        Status = RepairCardStatus.LegalExit;
        return Result.Updated;
    }
    public Result<Updated> MarkAsExitForPractice()
    {
        if (!IsEditable)
        {
            return RepairCardErrors.Readonly;
        }
        if (!CanTransitionTo(RepairCardStatus.ExitForPractice))
        {
            return RepairCardErrors.InvalidStateTransition(Status, RepairCardStatus.ExitForPractice);
        }
        Status = RepairCardStatus.ExitForPractice;
        return Result.Updated;
    }
    public Result<Updated> AssignAttendanceTime(AttendanceTime attendanceTime)
    {
        if (attendanceTime is null)
        {
            return RepairCardErrors.AttendanceTimeIsRequired;
        }
        
        AttendanceTime = attendanceTime;
        return Result.Updated;
    }
}