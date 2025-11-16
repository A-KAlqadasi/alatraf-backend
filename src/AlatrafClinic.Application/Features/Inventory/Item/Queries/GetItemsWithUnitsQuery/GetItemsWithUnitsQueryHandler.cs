using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.GetItemsWithUnitsQuery;

public sealed class GetItemsWithUnitsQueryHandler : IRequestHandler<GetItemsWithUnitsQuery, Result<List<ItemDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetItemsWithUnitsQueryHandler> _logger;

    public GetItemsWithUnitsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetItemsWithUnitsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<ItemDto>>> Handle(GetItemsWithUnitsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching all items with their units...");

        // نفترض أن الريبو يحتوي على دالة مخصصة لجلب العناصر مع الوحدات
        var items = await _unitOfWork.Items.GetAllWithUnitsAsync(cancellationToken);

        if (items is null || !items.Any())
        {
            _logger.LogWarning("No items found in the system.");
            return new List<ItemDto>(); // لا نرجع خطأ، فقط قائمة فارغة
        }

        var itemDtos = items.ToDtoList();

        _logger.LogInformation("Retrieved {Count} items with units successfully.", itemDtos.Count);

        return itemDtos;
    }
}
