using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;
using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.Diagnosises.InjuryReasons;
using AlatrafClinic.Domain.Diagnosises.InjurySides;
using AlatrafClinic.Domain.Diagnosises.InjuryTypes;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.RepairCards;
using AlatrafClinic.Domain.Sales;
using AlatrafClinic.Domain.Services.Tickets;
using AlatrafClinic.Domain.TherapyCards;

namespace AlatrafClinic.Domain.Diagnosises;

public class Diagnosis : AuditableEntity<int>
{
    public string DiagnosisText { get; private set; } = string.Empty;
    public DateOnly InjuryDate { get; private set; }

    // Links
    public int TicketId { get; private set; }
    public Ticket? Ticket { get; set; }
    public int PatientId { get; private set; }
    public Patient Patient { get; set; } = default!;
    public DiagnosisType DiagnoType { get; private set; }

    private readonly List<DiagnosisProgram> _diagnosisPrograms = new();
    public IReadOnlyCollection<DiagnosisProgram> DiagnosisPrograms => _diagnosisPrograms.AsReadOnly();
    private readonly List<DiagnosisIndustrialPart> _diagnosisIndustrialParts = new();
    public IReadOnlyCollection<DiagnosisIndustrialPart> DiagnosisIndustrialParts => _diagnosisIndustrialParts.AsReadOnly();
    
    public RepairCard? RepairCard { get; set; }
    public Sale? Sale { get; set; }
    public TherapyCard? TherapyCard { get; set; }
    private readonly List<InjuryReason> _injuryReasons = new();
    public IReadOnlyCollection<InjuryReason> InjuryReasons => _injuryReasons.AsReadOnly();
    private readonly List<InjurySide> _injurySides = new();
    public IReadOnlyCollection<InjurySide> InjurySides => _injurySides.AsReadOnly();
    private readonly List<InjuryType> _injuryTypes = new();
    public IReadOnlyCollection<InjuryType> InjuryTypes => _injuryTypes.AsReadOnly();

    private readonly List<Payment> _payments = new();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    
    private Diagnosis()
    {
    }
    public Diagnosis(
        int ticketId,
        string diagnosisText,
        DateOnly injuryDate,
        List<InjuryReason> injuryReasons,
        List<InjurySide> injurySides,
        List<InjuryType> injuryTypes,
        int patientId,
        DiagnosisType diagnosisType)
     {
        TicketId = ticketId;
        DiagnosisText = diagnosisText;
        InjuryDate = injuryDate;
        _injuryReasons = injuryReasons;
        _injurySides = injurySides;
        _injuryTypes = injuryTypes;
        PatientId = patientId;
        DiagnoType = diagnosisType;
    }
    public static Result<Diagnosis> Create(int ticketId,
        string diagnosisText,
        DateOnly injuryDate,
        List<InjuryReason> injuryReasons,
        List<InjurySide> injurySides,
        List<InjuryType> injuryTypes,
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
            if (injuryDate > AlatrafClinicConstants.TodayDate)
            {
                return DiagnosisErrors.InvalidInjuryDate;
            }
            if (injuryReasons == null || injuryReasons.Count == 0)
            {
                return DiagnosisErrors.InjuryReasonIsRequired;
            }
            if (injurySides == null || injurySides.Count == 0)
            {
                return DiagnosisErrors.InjurySideIsRequired;
            }
            if (injuryTypes == null || injuryTypes.Count == 0)
            {
                return DiagnosisErrors.InjuryTypeIsRequired;
            }
            
            if (patientId <= 0)
            {
                return DiagnosisErrors.InvalidPatientId;
            }

        return new Diagnosis(
            ticketId,
            diagnosisText,
            injuryDate,
            injuryReasons,
            injurySides,
            injuryTypes,
            patientId,
            diagnosisType);
    }
    public Result<Updated> Update(
        string diagnosisText,
        DateOnly injuryDate,
        List<InjuryReason> injuryReasons,
        List<InjurySide> injurySides,
        List<InjuryType> injuryTypes,
        DiagnosisType diagnosisType)
    {
        if (string.IsNullOrWhiteSpace(diagnosisText))
        {
            return DiagnosisErrors.DiagnosisTextIsRequired;
        }
        if (injuryDate > AlatrafClinicConstants.TodayDate)
        {
            return DiagnosisErrors.InvalidInjuryDate;
        }
        
        if (injuryReasons == null || injuryReasons.Count == 0)
        {
            return DiagnosisErrors.InjuryReasonIsRequired;
        }
        if (injurySides == null || injurySides.Count == 0)
        {
            return DiagnosisErrors.InjurySideIsRequired;
        }
        if (injuryTypes == null || injuryTypes.Count == 0)
        {
            return DiagnosisErrors.InjuryTypeIsRequired;
        }
      

        DiagnosisText = diagnosisText;
        InjuryDate = injuryDate;
        _injuryReasons.Clear();
        _injurySides.Clear();
        _injuryTypes.Clear();
        _injuryReasons.AddRange(injuryReasons);
        _injurySides.AddRange(injurySides);
        _injuryTypes.AddRange(injuryTypes);
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

    public Result<Updated> UpsertDiagnosisPrograms(List<(int medicalProgramId, int duration, string? notes)> diagnosisPrograms)
    {
        if (DiagnoType != DiagnosisType.Therapy)
        {
            return DiagnosisErrors.DiagnosisProgramAdditionOnlyForTherapyDiagnosis;
        }
        if (diagnosisPrograms is null || !diagnosisPrograms.Any())
        {
            return DiagnosisErrors.MedicalProgramsAreRequired;
        }

        _diagnosisPrograms.RemoveAll(dp => diagnosisPrograms.All(d => d.medicalProgramId != dp.MedicalProgramId));

        foreach (var (medicalProgramId, duration, notes) in diagnosisPrograms)
        {

            var existing = _diagnosisPrograms.FirstOrDefault(dp => dp.MedicalProgramId == medicalProgramId);

            if (existing != null)
            {
                var updated = existing.Update(this.Id, medicalProgramId, duration, notes);
                if (updated.IsError)
                {
                    return updated.Errors;
                }
            }
            else
            {
                var result = DiagnosisProgram.Create(this.Id, medicalProgramId, duration, notes);
                if (result.IsError)
                {
                    return result.Errors;
                }

                _diagnosisPrograms.Add(result.Value);
            }
        }
        return Result.Updated;
    }

    public Result<Updated> UpsertDiagnosisIndustrialParts(List<(int industrialPartUnitId, int quantity, decimal price)> incomingIndustrialParts)
    {
        if (DiagnoType != DiagnosisType.Limbs)
        {
            return DiagnosisErrors.IndustrialPartAdditionOnlyForLimbsDiagnosis;
        }
        if (incomingIndustrialParts.Count() <= 0)
        {
            return DiagnosisErrors.IndustrialPartsAreRequired;
        }

        _diagnosisIndustrialParts.RemoveAll(dip => incomingIndustrialParts.All(d => d.industrialPartUnitId != dip.IndustrialPartUnitId));

        foreach (var (industrialPartUnitId, quantity, price) in incomingIndustrialParts)
        {

            var existing = _diagnosisIndustrialParts.FirstOrDefault(dip => dip.IndustrialPartUnitId == industrialPartUnitId);

            if (existing != null)
            {
                var updated = existing.Update(this.Id, industrialPartUnitId, quantity, price);
                if (updated.IsError)
                {
                    return updated.Errors;
                }
            }
            else
            {
                var result = DiagnosisIndustrialPart.Create(this.Id, industrialPartUnitId, quantity, price);
                if (result.IsError)
                {
                    return result.Errors;
                }

                _diagnosisIndustrialParts.Add(result.Value);
            }
        }
        return Result.Updated;
    }
    public Result<Updated> AssignToSale(Sale sale)
    {
        if (DiagnoType != DiagnosisType.Sales)
        {
            return DiagnosisErrors.SaleAssignOnlyForSaleDiagnosis;
        }

        if (sale is null)
        {
            return DiagnosisErrors.SaleIsRequired;
        }
        Sale = sale;
        return Result.Updated;
    }
    
    public Result<Updated> AssignPayment(Payment payment)
    {
        if (payment is null)
        {
            return DiagnosisErrors.PaymentIsRequired;
        }

        var existingPayment = _payments.FirstOrDefault(p => p.Id == payment.Id);
        
        if (existingPayment != null)
        {
            return existingPayment.UpdateCore(payment.TicketId, payment.DiagnosisId, payment.TotalAmount, payment.PaymentReference);
        }
        else
        {
            _payments.Add(payment);
        }
        
        return Result.Updated;
    }

    public Result<Updated> AssignTherapyCard(TherapyCard therapyCard)
    {
        if(DiagnoType != DiagnosisType.Therapy)
        {
            return DiagnosisErrors.TherpyCardAssignmentOnlyForTherapyDiagnosis;
        }

        if(therapyCard is null)
        {
            return DiagnosisErrors.TherapyCardIsRequired;
        }
        TherapyCard = therapyCard;

        return Result.Updated;
    }
    public Result<Updated> AssignRepairCard(RepairCard repairCard)
    {
        if(DiagnoType != DiagnosisType.Limbs)
        {
            return DiagnosisErrors.RepairCardAssignmentOnlyForLimbsDiagnosis;
        }

        if(repairCard is null)
        {
            return DiagnosisErrors.RepairCardIsRequired;
        }
        
        RepairCard = repairCard;

        return Result.Updated;
    }
}