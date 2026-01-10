using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Stores.Dtos;
using AlatrafClinic.Application.Features.Inventory.Stores.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Stores;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetStoreByIdQuery;

public class GetStoreByIdQueryHandler : IRequestHandler<GetStoreByIdQuery, Result<StoreDto>>
{
    private readonly ILogger<GetStoreByIdQueryHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public GetStoreByIdQueryHandler(ILogger<GetStoreByIdQueryHandler> logger, IAppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<StoreDto>> Handle(GetStoreByIdQuery query, CancellationToken ct)
    {
        var store = await _dbContext.Stores
            .Include(s => s.StoreItemUnits)
                .ThenInclude(siu => siu.ItemUnit)
            .SingleOrDefaultAsync(s => s.Id == query.StoreId, ct);
        if (store is null)
        {
            _logger.LogWarning("Store {StoreId} not found", query.StoreId);
            return StoreErrors.StoreNotFound;
        }

        return store.ToDto();
    }
}
