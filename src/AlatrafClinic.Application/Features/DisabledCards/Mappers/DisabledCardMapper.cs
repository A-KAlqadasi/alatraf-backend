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
        var birthDate = entity.Patient?.Person?.Birthdate;
        return new DisabledCardDto
        {
            DisabledCardId = entity.Id,
            CardNumber = entity.CardNumber,
            ExpirationDate = entity.ExpirationDate,
            IssueDate = entity.IssueDate,
            IsExpired = entity.IsExpired,
            CardImagePath = entity.CardImagePath,
            FullName = entity.Patient?.Person?.FullName ?? string.Empty,
            Age = UtilityService.CalculateAge(birthDate ?? default, AlatrafClinicConstants.TodayDate),
            Gender = UtilityService.GenderToArabicString(entity.Patient?.Person?.Gender ?? true),
            PhoneNumber = entity.Patient?.Person?.Phone ?? string.Empty,
            PatientId = entity.PatientId
        };
    }
    public static List<DisabledCardDto> ToDtos(this IEnumerable<DisabledCard> entities)
    {
        return entities.Select(x => x.ToDto()).ToList();
    }
}