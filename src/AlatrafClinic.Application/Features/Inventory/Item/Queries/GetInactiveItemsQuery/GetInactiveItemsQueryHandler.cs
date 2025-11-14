using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.GetInactiveItemsQuery;

public sealed class GetInactiveItemsQueryHandler : IRequestHandler<GetInactiveItemsQuery, Result<List<ItemDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetInactiveItemsQueryHandler> _logger;

    public GetInactiveItemsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetInactiveItemsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<ItemDto>>> Handle(GetInactiveItemsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching inactive items...");

        var inactiveItems = await _unitOfWork.Item.GetInactiveAsync(cancellationToken);

        if (inactiveItems is null || !inactiveItems.Any())
        {
            _logger.LogWarning("No inactive items found.");
            return new List<ItemDto>(); // إرجاع قائمة فارغة بدون خطأ
        }

        var itemDtos = inactiveItems.ToDtoList();
        _logger.LogInformation("Retrieved {Count} inactive items successfully.", itemDtos.Count);

        return itemDtos;
    }
}
