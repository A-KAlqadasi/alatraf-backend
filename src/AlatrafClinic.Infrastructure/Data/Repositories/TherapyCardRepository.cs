
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.TherapyCards;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class TherapyCardRepository : GenericRepository<TherapyCard, int>, ITherapyCardRepository
{
    public TherapyCardRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<TherapyCard?> GetLastActiveTherapyCardByPatientIdAsync(int patientId, CancellationToken ct)
    {
        return await _dbContext.TherapyCards
            .OrderByDescending(tc => tc.CreatedAtUtc.DateTime)
            .FirstOrDefaultAsync(tc => tc.Diagnosis.PatientId == patientId && tc.IsActive, ct);
    }
}