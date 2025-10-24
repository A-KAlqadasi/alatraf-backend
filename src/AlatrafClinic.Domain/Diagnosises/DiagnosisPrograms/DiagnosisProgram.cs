using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
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
    //public TherapyCard? TherapyCard { get; set; }

    public ICollection<SessionProgram> SessionPrograms { get; set; } = new List<SessionProgram>();
    private DiagnosisProgram()
    {
    }
    public DiagnosisProgram(
        int? diagnosisId,
        int? medicalProgramId,
        int? duration,
        string? notes,
        int? therapyCardId)
    {
        DiagnosisId = diagnosisId;
        MedicalProgramId = medicalProgramId;
        Duration = duration;
        Notes = notes;
        TherapyCardId = therapyCardId;
    }
    public static Result<DiagnosisProgram> Create(
        int? diagnosisId,
        int? medicalProgramId,
        int? duration,
        string? notes,
        int? therapyCardId = null)
    {
        // Validation
        if (diagnosisId is null)
        {
            return DiagnosisProgramErrors.DiagnosisIdIsRequired;
        }
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
            diagnosisId,
            medicalProgramId,
            duration,
            notes,
            therapyCardId);
    }

    public Result<Updated> Update(
        int? diagnosisId,
        int? medicalProgramId,
        int? duration,
        string? notes,
        int? therapyCardId = null)
    {
        if (diagnosisId is null)
        {
            return DiagnosisProgramErrors.DiagnosisIdIsRequired;
        }
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

        DiagnosisId = diagnosisId;
        MedicalProgramId = medicalProgramId;
        Duration = duration;
        Notes = notes;
        TherapyCardId = therapyCardId;

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