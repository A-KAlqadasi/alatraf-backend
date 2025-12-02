using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.WoundedCards;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class WoundedCardRepository : GenericRepository<WoundedCard, int>, IWoundedCardRepository
{
    public WoundedCardRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> IsExistAsync(string cardNumber, CancellationToken ct = default)
    {
        return await dbContext.WoundedCards
            .AsNoTracking()
            .AnyAsync(wc => wc.CardNumber == cardNumber, ct);
    }

    public async Task<WoundedCard?> GetWoundedCardByNumberAsync(string cardNumber, CancellationToken ct = default)
    {
        return await dbContext.WoundedCards
            .AsNoTracking()
            .FirstOrDefaultAsync(wc => wc.CardNumber == cardNumber, ct);
    }
}