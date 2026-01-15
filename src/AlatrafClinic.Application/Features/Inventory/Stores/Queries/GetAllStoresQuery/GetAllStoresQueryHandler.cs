using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Stores.Dtos;
using AlatrafClinic.Application.Features.Inventory.Stores.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetAllStoresQuery;

public class GetAllStoresQueryHandler : IRequestHandler<GetAllStoresQuery, Result<List<StoreDto>>>
{
    private readonly ILogger<GetAllStoresQueryHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public GetAllStoresQueryHandler(ILogger<GetAllStoresQueryHandler> logger, IAppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<List<StoreDto>>> Handle(GetAllStoresQuery request, CancellationToken ct)
    {
        var stores = await _dbContext.Stores
            .AsNoTracking()
            .ToListAsync(ct);
        return stores.ToDtos();
    }
}
