using AlatrafClinic.Domain.TherapyCards;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface ITherapyCardRepository : IGenericRepository<TherapyCard, int>
{
    Task<TherapyCard?> GetLastActiveTherapyCardByPatientIdAsync(int patientId, CancellationToken ct);
}