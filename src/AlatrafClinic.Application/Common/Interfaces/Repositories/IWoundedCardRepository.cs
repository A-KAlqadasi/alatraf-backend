using AlatrafClinic.Domain.WoundedCards;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface IWoundedCardRepository : IGenericRepository<WoundedCard, int>
{
    Task<bool> IsExistAsync(string cardNumber, CancellationToken ct = default);
    Task<WoundedCard?> GetWoundedCardByNumberAsync(string cardNumber, CancellationToken ct= default);
}