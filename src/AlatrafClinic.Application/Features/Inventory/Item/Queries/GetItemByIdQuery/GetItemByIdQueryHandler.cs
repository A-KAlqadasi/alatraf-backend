using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.GetItemByIdQuery;

public sealed class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, Result<ItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetItemByIdQueryHandler> _logger;

    public GetItemByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetItemByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ItemDto>> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching item with Id: {Id}", request.Id);

        var item = await _unitOfWork.Items.GetByIdAsync(request.Id, cancellationToken);

        if (item is null)
        {
            _logger.LogWarning("Item with Id {Id} not found.", request.Id);
            return ItemErrors.NotFound;
        }

        // تأكد من أن الوحدات تم تحميلها (في حال استخدام lazy loading أو need explicit Include)
        if (item.ItemUnits is null || !item.ItemUnits.Any())
        {
            _logger.LogInformation("Item with Id {Id} has no associated units.", request.Id);
        }

        var dto = item.ToDto();

        _logger.LogInformation("Item with Id {Id} retrieved successfully.", request.Id);

        return dto;
    }
}
