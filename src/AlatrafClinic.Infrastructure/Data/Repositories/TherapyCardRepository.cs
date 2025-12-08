
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.TherapyCards;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class TherapyCardRepository : GenericRepository<TherapyCard, int>, ITherapyCardRepository
{
    public TherapyCardRepository(AlatrafClinicDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<TherapyCard?> GetLastActiveTherapyCardByPatientIdAsync(int patientId, CancellationToken ct)
    {
        return await dbContext.TherapyCards
            .OrderByDescending(tc => tc.CreatedAtUtc.DateTime)
            .FirstOrDefaultAsync(tc => tc.Diagnosis.PatientId == patientId && tc.IsActive, ct);
    }

    public async new Task<TherapyCard?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await dbContext.TherapyCards
            .AsNoTracking()
            .Include(tc => tc.Diagnosis)
                .ThenInclude(d => d.Patient)!.ThenInclude(p=> p.Person)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(ir=> ir.InjuryReasons)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(injs=> injs.InjurySides)
            .Include(tc => tc.Diagnosis)
                .ThenInclude(it=> it.InjuryTypes)
            .Include(tc => tc.Diagnosis).ThenInclude(p=> p.Payments)
            .Include(tc => tc.DiagnosisPrograms)!.ThenInclude(dp => dp.MedicalProgram)
            .Include(tc => tc.Sessions)!
                .ThenInclude(s => s.SessionPrograms)!
                    .ThenInclude(sp => sp.DiagnosisProgram)!.ThenInclude(dp => dp.MedicalProgram)   
            .FirstOrDefaultAsync(tc => tc.Id == id, ct);
    }
}