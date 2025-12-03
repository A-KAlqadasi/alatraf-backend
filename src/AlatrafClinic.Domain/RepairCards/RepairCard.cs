using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;
using AlatrafClinic.Domain.ExitCards;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.RepairCards.DeliveryTimes;
using AlatrafClinic.Domain.RepairCards.Enums;
using AlatrafClinic.Domain.RepairCards.Orders;


namespace AlatrafClinic.Domain.RepairCards;

public class RepairCard : AuditableEntity<int>
{
    public RepairCardStatus Status { get; private set; }
    public bool IsActive { get; private set; }
    public int DiagnosisId { get; private set; }
    public Diagnosis Diagnosis { get; set; } = default!;

    public bool IsPaid => Diagnosis.Payments.Any(p => p.DiagnosisId == DiagnosisId && p.IsCompleted);
    public Payment? Payment => Diagnosis.Payments.FirstOrDefault(p => p.DiagnosisId == DiagnosisId);

    public ExitCard? ExitCard { get; set; }
    public string? Notes { get; private set; }
    public decimal TotalCost => _diagnosisIndustrialParts.Sum(part => part.Price * part.Quantity);

    // Navigation
    public DeliveryTime? DeliveryTime { get;  set; }
    public bool IsLate => Status is RepairCardStatus.InProgress && DeliveryTime?.DeliveryDate.Date < DateTime.Now.Date;

    private readonly List<Order> _orders = new();
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();
    private readonly List<DiagnosisIndustrialPart> _diagnosisIndustrialParts = new();
    public IReadOnlyCollection<DiagnosisIndustrialPart> DiagnosisIndustrialParts => _diagnosisIndustrialParts.AsReadOnly();

    private RepairCard() { }

    private RepairCard(int diagnosisId, List<DiagnosisIndustrialPart> diagnosisIndustrialParts,  RepairCardStatus status, string? notes = null, bool isActive = true)
    {
        DiagnosisId = diagnosisId;
        IsActive = isActive;
        Status = status;
        Notes = notes;
        _diagnosisIndustrialParts = diagnosisIndustrialParts;
    }

    public static Result<RepairCard> Create(int diagnosisId, List<DiagnosisIndustrialPart> diagnosisIndustrialParts, string? notes = null)
    {
        if (diagnosisId <= 0)
        {
            return RepairCardErrors.InvalidDiagnosisId;
        }

        return new RepairCard(diagnosisId, diagnosisIndustrialParts, RepairCardStatus.New, notes: notes);
    }

    public bool IsEditable => IsActive && Status is not RepairCardStatus.LegalExit or RepairCardStatus.IllegalExit;

    public Result<Updated> UpsertIndustrialParts(List<DiagnosisIndustrialPart> newIndustrialParts)
    {
        if (Status != RepairCardStatus.New)
        {
            return RepairCardErrors.Readonly;
        }

        _diagnosisIndustrialParts.Clear();
        _diagnosisIndustrialParts.AddRange(newIndustrialParts);

        return Result.Updated;
    }
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
    public Result<Updated> AssignDeliveryTime(DateTime deliveryDate, string? note)
    {
        if (!IsEditable)
        {
            return RepairCardErrors.Readonly;
        }

        var deliveryTimeResult = DeliveryTime.Create(Id, deliveryDate, note);
        if (deliveryTimeResult.IsError)
        {
            return deliveryTimeResult.Errors;
        }

        DeliveryTime = deliveryTimeResult.Value;
        return Result.Updated;
    }

    public Result<Updated> AssignOrder(Order order)
    {
        if (!IsEditable) return RepairCardErrors.Readonly;

        if (order is null) return RepairCardErrors.InvalidOrder;

        if (_orders.Any(o => o.Id == order.Id)) return RepairCardErrors.OrderAlreadyExists;


        _orders.Add(order);

        return Result.Updated;
    }
    
    public Result<Updated> AssignExitCard(string? notes)
    {
        if (ExitCard is not null)
        {
            return RepairCardErrors.ExitCardAlreadyAssigned;
        }

        var patientId = Diagnosis.PatientId;

        var exitCardResult = ExitCard.Create(patientId, notes);
        if (exitCardResult.IsError)
        {
            return exitCardResult.Errors;
        }

        ExitCard = exitCardResult.Value;

        ExitCard.AssignRepairCard(this);

        return Result.Updated;
    }
}