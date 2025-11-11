using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Domain.RepairCards;

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
            CardStatus = repairCard.Status,
            DeliveryDate = repairCard.DeliveryTime?.DeliveryDate ?? repairCard.CreatedAtUtc.ToLocalTime().Date,
            DiagnosisIndustrialParts = repairCard.DiagnosisIndustrialParts.ToDtos()
        };
    }
    public static List<RepairCardDto> ToDtos(this List<RepairCard> repairCards)
    {
        return [..repairCards.Select(x=> x.ToDto())];
    }
}