using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;
using AlatrafClinic.Domain.TherapyCards.Sessions;

namespace AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;

public class DiagnosisProgram : AuditableEntity<int>
{
    public int DiagnosisId { get; private set; }
    public Diagnosis? Diagnosis { get; set; }
    public int MedicalProgramId { get; private set; }
    public MedicalProgram? MedicalProgram { get; set; }
    public int Duration { get; private set; }
    public string? Notes { get; private set; } = null;
    public int? TherapyCardId { get; private set; } = null;
    public TherapyCard? TherapyCard { get; set; }

    public ICollection<SessionProgram> SessionPrograms { get; set; } = new List<SessionProgram>();
    private DiagnosisProgram()
    {
    }
    public DiagnosisProgram(
        int diagnosisId,
        int medicalProgramId,
        int duration,
        string? notes)
    {
        DiagnosisId = diagnosisId;
        MedicalProgramId = medicalProgramId;
        Duration = duration;
        Notes = notes;
    }
    public static Result<DiagnosisProgram> Create(
        int diagnosisId,
        int medicalProgramId,
        int duration,
        string? notes)
    {
        
        if (medicalProgramId <= 0)
        {
            return DiagnosisProgramErrors.InvalidMedicalProgramId;
        }
        if (duration <= 0)
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
            notes);
    }

    public Result<Updated> Update(
        int diagnosisId,
        int medicalProgramId,
        int duration,
        string? notes)
    {
        if (diagnosisId <= 0)
        {
            return DiagnosisProgramErrors.InvalidDiagnosisId;
        }
        if (medicalProgramId <= 0)
        {
            return DiagnosisProgramErrors.InvalidMedicalProgramId;
        }
        if (duration <= 0)
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

        return Result.Updated;
    }

}