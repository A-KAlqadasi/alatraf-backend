using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Common.Interfaces.Repositories.Inventory;
using AlatrafClinic.Domain.Inventory.Items;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Infrastructure.Data.Repositories.Inventory;

public class ItemRepository : GenericRepository<Item, int>, IItemRepository
{
    public ItemRepository(AlatrafClinicDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<ItemUnit?> GetByIdAndUnitIdAsync(int id, int unitId, CancellationToken ct)
    {
        return await dbContext.ItemUnits
            .AsNoTracking()
            .SingleOrDefaultAsync(iu => iu.ItemId == id && iu.UnitId == unitId, ct);
    }

    public async Task<IEnumerable<Item>> GetAllWithUnitsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Items
            .AsNoTracking()
            .Include(i => i.ItemUnits)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Item>> GetInactiveAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Items
            .AsNoTracking()
            .Where(i => !i.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<Item?> GetByIdWithUnitsAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Items
            .AsQueryable()
            .Include(i => i.ItemUnits)
            .SingleOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Item>> SearchAsync(ItemSearchSpec spec, CancellationToken cancellationToken)
    {
        var query = dbContext.Items.AsQueryable();

        if (!string.IsNullOrWhiteSpace(spec.Keyword))
        {
            var kw = spec.Keyword.Trim();
            query = query.Where(i => i.Name.Contains(kw));
        }

        if (spec.BaseUnitId.HasValue)
            query = query.Where(i => i.BaseUnitId == spec.BaseUnitId.Value);

        if (spec.IsActive.HasValue)
            query = query.Where(i => i.IsActive == spec.IsActive.Value);

        // filter by unit id or price using ItemUnits
        if (spec.UnitId.HasValue || spec.MinPrice.HasValue || spec.MaxPrice.HasValue)
        {
            if (spec.UnitId.HasValue)
            {
                var uid = spec.UnitId.Value;
                query = query.Where(i => i.ItemUnits.Any(iu => iu.UnitId == uid));
            }

            if (spec.MinPrice.HasValue)
            {
                var min = spec.MinPrice.Value;
                query = query.Where(i => i.ItemUnits.Any(iu => iu.Price >= min));
            }

            if (spec.MaxPrice.HasValue)
            {
                var max = spec.MaxPrice.Value;
                query = query.Where(i => i.ItemUnits.Any(iu => iu.Price <= max));
            }
        }

        var total = await query.CountAsync(cancellationToken);

        // ordering
        if (!string.IsNullOrWhiteSpace(spec.SortBy))
        {
            var dir = spec.SortDir?.ToLowerInvariant() == "desc";
            if (spec.SortBy.Equals("name", StringComparison.OrdinalIgnoreCase))
                query = dir ? query.OrderByDescending(i => i.Name) : query.OrderBy(i => i.Name);
            else
                query = query.OrderBy(i => i.Id);
        }
        else
        {
            query = query.OrderBy(i => i.Name);
        }

        var skip = (spec.Page - 1) * spec.PageSize;
        var items = await query.Skip(skip).Take(spec.PageSize).ToListAsync(cancellationToken);

        return new PagedResult<Item>(items, total, spec.Page, spec.PageSize);
    }
}
