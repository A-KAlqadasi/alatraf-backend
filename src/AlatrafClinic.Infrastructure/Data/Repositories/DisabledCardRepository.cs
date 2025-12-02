using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.DisabledCards;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories;

public class DisabledCardRepository : GenericRepository<DisabledCard, int>, IDisabledCardRepository
{
    public DisabledCardRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<DisabledCard?> GetDisabledCardByNumber(string cardNumber, CancellationToken ct = default)
    {
        return await dbContext.DisabledCards
            .FirstOrDefaultAsync(dc => dc.CardNumber == cardNumber, ct);
    }

    public async Task<bool> IsExistAsync(string cardNumber, CancellationToken ct = default)
    {
        return await dbContext.DisabledCards
            .AnyAsync(dc => dc.CardNumber == cardNumber, ct);
    }
}