using AlatrafClinic.Domain.DisabledCards;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IDisabledCardRepository : IGenericRepository<DisabledCard, int>
{
    Task<DisabledCard?> GetDisabledCardByNumber(string cardNumber, CancellationToken ct = default);
    Task<bool> IsExistAsync(string cardNumber, CancellationToken ct = default);
}