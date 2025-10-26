using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;
using AlatrafClinic.Domain.TherapyCards.Sessions;

namespace AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;

public class DiagnosisProgram : AuditableEntity<int>
{
    public int? DiagnosisId { get; set; }
    public Diagnosis? Diagnosis { get; set; }
    public int? MedicalProgramId { get; set; }
    public MedicalProgram? MedicalProgram { get; set; }
    public int? Duration { get; set; }
    public string? Notes { get; set; }
    public int? TherapyCardId { get; set; }
    public TherapyCard? TherapyCard { get; set; }

    public ICollection<SessionProgram> SessionPrograms { get; set; } = new List<SessionProgram>();
    private DiagnosisProgram()
    {
    }
    public DiagnosisProgram(
        int? medicalProgramId,
        int? duration,
        string? notes)
    {
        MedicalProgramId = medicalProgramId;
        Duration = duration;
        Notes = notes;
    }
    public static Result<DiagnosisProgram> Create(
        int? medicalProgramId,
        int? duration,
        string? notes)
    {
       
        if (medicalProgramId is null)
        {
            return DiagnosisProgramErrors.MedicalProgramIdIsRequired;
        }
        if (duration is not null && duration <= 0)
        {
            return DiagnosisProgramErrors.InvalidDuration;
        }
        if (notes is not null && notes.Length > 1000)
        {
            return DiagnosisProgramErrors.NotesTooLong;
        }

        return new DiagnosisProgram(
            medicalProgramId,
            duration,
            notes);
    }

    public Result<Updated> Update(
        int? medicalProgramId,
        int? duration,
        string? notes)
    {
        if (medicalProgramId is null)
        {
            return DiagnosisProgramErrors.MedicalProgramIdIsRequired;
        }
        if (duration is not null && duration <= 0)
        {
            return DiagnosisProgramErrors.InvalidDuration;
        }
        if (notes is not null && notes.Length > 1000)
        {
            return DiagnosisProgramErrors.NotesTooLong;
        }
        
        MedicalProgramId = medicalProgramId;
        Duration = duration;
        Notes = notes;

        return Result.Updated;
    }

    public Result<Updated> AssignTherapyCard(int therapyCardId)
    {
        if (therapyCardId <= 0)
        {
            return DiagnosisProgramErrors.TherapyCardIdIsRequired;
        }

        TherapyCardId = therapyCardId;
        return Result.Updated;
    }
}