using AlatrafClinic.Application.Features.WoundedCards.Dtos;
using AlatrafClinic.Domain.WoundedCards;

namespace AlatrafClinic.Application.Features.WoundedCards.Mappers;

public static class WoundedCardMapper
{
    public static WoundedCardDto ToDto(this WoundedCard entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new WoundedCardDto
        {
            WoundedCardId = entity.Id,
            CardNumber = entity.CardNumber,
            ExpirationDate = entity.Expiration,
            IsExpired = entity.IsExpired,
            CardImagePath = entity.CardImagePath,
            FullName = entity.Patient?.Person?.FullName ?? string.Empty,
            PatientId = entity.PatientId
        };
    }
    public static List<WoundedCardDto> ToDtos(this IEnumerable<WoundedCard> entities)
    {
        return entities.Select(x => x.ToDto()).ToList();
    }
}