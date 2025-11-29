using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.TherapyCards.Enums;
using AlatrafClinic.Domain.TherapyCards.TherapyCardTypePrices;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class TherapyCardTypePriceRepository : GenericRepository<TherapyCardTypePrice, int>, ITherapyCardTypePriceRepository
{
    public TherapyCardTypePriceRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<decimal?> GetSessionPriceByTherapyCardTypeAsync(TherapyCardType type, CancellationToken ct)
    {
        var therapyCardTypePrice = await _dbContext.TherapyCardTypePrices
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Type == type, ct);

        return therapyCardTypePrice?.SessionPrice;
    }
}