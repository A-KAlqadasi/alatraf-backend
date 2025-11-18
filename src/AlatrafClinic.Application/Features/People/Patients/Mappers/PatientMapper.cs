

using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Application.Features.People.Persons.Mappers;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Patients.Cards.DisabledCards;
using AlatrafClinic.Domain.Patients.Cards.WoundedCards;

namespace AlatrafClinic.Application.Features.People.Patients.Mappers;

public static class PatientMapper 
{
    public static PatientDto ToDto(this Patient entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new PatientDto
        {
            PatientId = entity.Id,
            PersonId = entity.PersonId,
            PersonDto = entity.Person!.ToDto(),
            PatientType = entity.PatientType,
            AutoRegistrationNumber = entity.AutoRegistrationNumber
        };
    }

    public static List<PatientDto> ToDtos(this IEnumerable<Patient> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }
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