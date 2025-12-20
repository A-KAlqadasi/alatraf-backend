using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.WoundedCards.Dtos;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.WoundedCards;

namespace AlatrafClinic.Application.Features.WoundedCards.Mappers;

public static class WoundedCardMapper
{
    public static WoundedCardDto ToDto(this WoundedCard entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        var birthDate = entity.Patient?.Person?.Birthdate;
        return new WoundedCardDto
        {
            WoundedCardId = entity.Id,
            CardNumber = entity.CardNumber,
            IssueDate = entity.IssueDate,
            ExpirationDate = entity.ExpirationDate,
            IsExpired = entity.IsExpired,
            CardImagePath = entity.CardImagePath,
            FullName = entity.Patient?.Person?.FullName ?? string.Empty,
            Age = UtilityService.CalculateAge(birthDate ?? default, AlatrafClinicConstants.TodayDate),
            Gender = UtilityService.GenderToArabicString(entity.Patient?.Person?.Gender ?? true),
            PhoneNumber = entity.Patient?.Person?.Phone ?? string.Empty,
            PatientId = entity.PatientId
        };
    }
    public static List<WoundedCardDto> ToDtos(this IEnumerable<WoundedCard> entities)
    {
        return entities.Select(x => x.ToDto()).ToList();
    }
}