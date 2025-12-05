using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Stores.Dtos;
using AlatrafClinic.Application.Features.Inventory.Stores.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Stores;

using MediatR;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetStoreByIdQuery;

public class GetStoreByIdQueryHandler : IRequestHandler<GetStoreByIdQuery, Result<StoreDto>>
{
    private readonly ILogger<GetStoreByIdQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetStoreByIdQueryHandler(ILogger<GetStoreByIdQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<StoreDto>> Handle(GetStoreByIdQuery query, CancellationToken ct)
    {
        var store = await _unitOfWork.Stores.GetByIdWithItemUnitsAsync(query.StoreId, ct);
        if (store is null)
        {
            _logger.LogWarning("Store {StoreId} not found", query.StoreId);
            return StoreErrors.StoreNotFound;
        }

        return store.ToDto();
    }
}
