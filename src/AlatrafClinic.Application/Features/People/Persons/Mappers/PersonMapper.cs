
using AlatrafClinic.Application.Features.People.Persons.Dtos;
using AlatrafClinic.Domain.People;

namespace AlatrafClinic.Application.Features.People.Persons.Mappers;

    public static class PersonMapper
    {
        public static PersonDto ToDto(this Person entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            return new PersonDto
            {
                PersonId = entity.Id,
                Fullname = entity.Fullname ?? string.Empty,
                Birthdate = entity.Birthdate,
                Phone = entity.Phone,
                NationalNo = entity.NationalNo,
                Address = entity.Address
            };
        }

        public static List<PersonDto> ToDtos(this IEnumerable<Person> entities)
        {
                  return [.. entities.Select(e => e.ToDto())];

        }
    }
