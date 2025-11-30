using AlatrafClinic.Application.Features.DisabledCards.Dtos;
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
            ExpirationDate = entity.ExpirationDate,
            IsExpired = entity.IsExpired,
            CardImagePath = entity.CardImagePath,
            FullName = entity.Patient?.Person?.FullName ?? string.Empty,
            PatientId = entity.PatientId
        };
    }
    public static List<DisabledCardDto> ToDtos(this IEnumerable<DisabledCard> entities)
    {
        return entities.Select(x => x.ToDto()).ToList();
    }
}