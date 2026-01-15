using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.DisabledCards.Dtos;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.DisabledCards;

namespace AlatrafClinic.Application.Features.DisabledCards.Mappers;

public static class DisabledCardMapper
{
    public static DisabledCardDto ToDto(this DisabledCard entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new DisabledCardDto
        {
            DisabledCardId = entity.Id,
            CardNumber = entity.CardNumber,
            DisabilityType = entity.DisabilityType,
            IssueDate = entity.IssueDate,
            CardImagePath = entity.CardImagePath,
            FullName = entity.Patient?.Person?.FullName ?? string.Empty,
            Age = entity.Patient?.Person?.Age ?? 0,
            Gender = UtilityService.GenderToArabicString(entity.Patient?.Person?.Gender ?? true),
            PhoneNumber = entity.Patient?.Person?.Phone ?? string.Empty,
            Address = entity.Patient?.Person?.Address.Name ?? string.Empty,
            PatientId = entity.PatientId
        };
    }
    public static List<DisabledCardDto> ToDtos(this IEnumerable<DisabledCard> entities)
    {
        return entities.Select(x => x.ToDto()).ToList();
    }
}