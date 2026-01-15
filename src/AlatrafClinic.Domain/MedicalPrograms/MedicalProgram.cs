using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections;
using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;

namespace AlatrafClinic.Domain.MedicalPrograms;

public class MedicalProgram : AuditableEntity<int>
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; } 
    public int? SectionId { get; set; }
    public Section? Section { get; set; }
    public ICollection<DiagnosisProgram> DiagnosisPrograms { get; set; } = new List<DiagnosisProgram>();

    private MedicalProgram() { }

    private MedicalProgram(string name, string? description, int? sectionId)
    {
        Name = name;
        SectionId = sectionId;
        Description = description;
    }
    public static Result<MedicalProgram> Create(string name, string? description = null, int? sectionId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return MedicalProgramErrors.NameIsRequired;
        }

        return new MedicalProgram(name, description, sectionId);
    }
    public Result<Updated> Update(string name, string? description = null, int? sectionId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return MedicalProgramErrors.NameIsRequired;
        }

        Name = name;
        SectionId = sectionId;
        Description = description;
        return Result.Updated;
    }
}