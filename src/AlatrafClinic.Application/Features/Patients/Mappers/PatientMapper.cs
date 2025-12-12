using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.Patients.Dtos;
using AlatrafClinic.Application.Features.Patients.Mappers;
using AlatrafClinic.Application.Features.People.Mappers;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Patients.Enums;

namespace AlatrafClinic.Application.Features.Patients.Mappers;

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
            PatientType = entity.PatientType.ToArabicPatientType(),
        };
    }

    public static List<PatientDto> ToDtos(this IEnumerable<Patient> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }
    public static string ToArabicPatientType(this PatientType type)
    {
        switch (type)
        {
            case PatientType.Normal : return "مدني";
            case PatientType.Wounded : return "جريح";
            case PatientType.Disabled : return "معاق";
            
            default:
                throw new Exception($"Invalid patient type: {type.ToString()}");
        }
    }
}