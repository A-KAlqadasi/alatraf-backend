using AlatrafClinic.Application.Features.MedicalPrograms.Dtos;
using AlatrafClinic.Domain.MedicalPrograms;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Mappers;

public static class MedicalProgramMapper
{
    public static MedicalProgramDto ToDto(this MedicalProgram medicalProgram)
    {
        return new MedicalProgramDto
        {
            Id = medicalProgram.Id,
            Name = medicalProgram.Name,
            Description = medicalProgram.Description,
            SectionId = medicalProgram.SectionId,
            SectionName = medicalProgram.Section?.Name
        };
    }

    public static List<MedicalProgramDto> ToDtos(this IEnumerable<MedicalProgram> medicalPrograms)
    {
        return medicalPrograms.Select(mp => mp.ToDto()).ToList();
    }
}