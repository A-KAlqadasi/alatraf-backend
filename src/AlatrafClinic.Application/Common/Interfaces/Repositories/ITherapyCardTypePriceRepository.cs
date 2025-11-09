using AlatrafClinic.Domain.TherapyCards.Enums;
using AlatrafClinic.Domain.TherapyCards.TherapyCardTypePrices;

namespace AlatrafClinic.Application.Common.Interfaces.Repositories;

public interface ITherapyCardTypePriceRepository : IGenericRepository<TherapyCardTypePrice, int>
{
    Task<decimal?> GetSessionPriceByTherapyCardTypeAsync(TherapyCardType type, CancellationToken ct);
}