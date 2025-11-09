using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.TherapyCards;

namespace AlatrafClinic.Application.Features.TherapyCards.Mappers;

public static class TherapyCardMapper
{
    public static TherapyCardDto ToDto(this TherapyCard entity)
    {
        return new TherapyCardDto
        {
            TherapyCardId = entity.Id,
            Diagnosis = entity.Diagnosis!.ToDto(),
            IsActive = entity.IsActive,
            NumberOfSessions = entity.NumberOfSessions,
            ProgramStartDate = entity.ProgramStartDate,
            TherapyCardType = entity.Type,
            CardStatus = entity.CardStatus,
            ProgramEndDate = entity.ProgramEndDate,
            Notes = entity.Notes,
            Programs = entity.Diagnosis!.DiagnosisPrograms?.ToDtos()
        };
    }
    public static List<TherapyCardDto> ToDtos(this IEnumerable<TherapyCard> therapyCards)
    {
        return therapyCards.Select(t => t.ToDto()).ToList();
    }
}