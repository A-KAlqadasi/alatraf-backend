using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;
using AlatrafClinic.Domain.Organization.Sections;

namespace AlatrafClinic.Domain.TherapyCards.MedicalPrograms;

public class MedicalProgram : AuditableEntity<int>
{
    public string? Name { get; set; }
    public int? SectionId { get; set; }
    public Section? Section { get; set; }
    public ICollection<DiagnosisProgram> DiagnosisPrograms { get; set; } = new List<DiagnosisProgram>();

    private MedicalProgram() { }

    private MedicalProgram(string name, int? sectionId)
    {
        Name = name;
        SectionId = sectionId;
    }
    public static Result<MedicalProgram> Create(string name, int? sectionId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return MedicalProgramErrors.NameIsRequired;
        }

        return new MedicalProgram(name, sectionId);
    }
    public Result<Updated> Update(string name, int? sectionId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return MedicalProgramErrors.NameIsRequired;
        }

        Name = name;
        SectionId = sectionId;
        return Result.Updated;
    }
}