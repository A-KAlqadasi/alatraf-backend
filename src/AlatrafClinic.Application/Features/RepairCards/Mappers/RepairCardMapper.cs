using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Domain.RepairCards;
using AlatrafClinic.Domain.RepairCards.Enums;

namespace AlatrafClinic.Application.Features.RepairCards.Mappers;

public static class RepairCardMapper
{
    public static RepairCardDto ToDto(this RepairCard repairCard)
    {
        ArgumentNullException.ThrowIfNull(repairCard);
        return new RepairCardDto
        {
            RepairCardId = repairCard.Id,
            Diagnosis = repairCard.Diagnosis.ToDto(),
            IsActive = repairCard.IsActive,
            IsLate = repairCard.IsLate,
            CardStatus = repairCard.Status.ToArabicRepairCardStatus(),
            TotalCost = repairCard.TotalCost,
            DeliveryDate = repairCard.DeliveryTime?.DeliveryDate ?? repairCard.CreatedAtUtc.ToLocalTime().Date,
            DiagnosisIndustrialParts = repairCard.DiagnosisIndustrialParts.ToDtos()
            
        };
    }
    public static List<RepairCardDto> ToDtos(this List<RepairCard> repairCards)
    {
        return [..repairCards.Select(x=> x.ToDto())];
    }
    public static RepairCardDiagnosisDto ToDiagnosisDto(this RepairCard entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new RepairCardDiagnosisDto
        {
            RepairCardId = entity.Id,
            TicketId = entity.Diagnosis.TicketId,
            PatientId = entity.Diagnosis.PatientId,
            PatientName = entity.Diagnosis.Patient.Person.FullName,
            Gender  = entity.Diagnosis.Patient.Person.Gender ? "ذكر" : "أنثى",
            Age = DateTime.Now.Year - entity.Diagnosis.Patient.Person.Birthdate.Year,
            DiagnosisId = entity.DiagnosisId,
            DiagnosisText = entity.Diagnosis.DiagnosisText,
            InjuryDate  = entity.Diagnosis.InjuryDate,
            DiagnosisType = entity.Diagnosis.DiagnoType.ToArabicDiagnosisType(),
            InjuryReasons = entity.Diagnosis.InjuryReasons.ToDtos(),
            InjurySides = entity.Diagnosis.InjurySides.ToDtos(),
            InjuryTypes = entity.Diagnosis.InjuryTypes.ToDtos(),
            CardStatus = entity.Status.ToArabicRepairCardStatus(),
            DiagnosisIndustrialParts = entity.DiagnosisIndustrialParts.ToDtos(),
            TotalCost = entity.TotalCost
        };
    }

    public static List<RepairCardDiagnosisDto> ToDiagnosisDtos(this List<RepairCard> entities)
    {
        return [..entities.Select(x => x.ToDiagnosisDto())];
    }

    public static string ToArabicRepairCardStatus(this RepairCardStatus status)
    {
        return status switch
        {
            RepairCardStatus.New => "جديد",
            RepairCardStatus.InProgress => "قيد التنفيذ",
            RepairCardStatus.Completed => "مكتمل",
            RepairCardStatus.AssignedToTechnician => "مُسند إلى فني",
            RepairCardStatus.InTraining => "في التدريب",
            RepairCardStatus.ExitForPractice => "خروج للممارسة",
            RepairCardStatus.IllegalExit => "خروج غير قانوني",
            RepairCardStatus.LegalExit => "تم التسليم",
            _ => "غير معروف"
        };
    }
}