using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.GetItemByIdQuery;

public sealed class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, Result<ItemDto>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetItemByIdQueryHandler> _logger;

    public GetItemByIdQueryHandler(IAppDbContext dbContext, ILogger<GetItemByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<ItemDto>> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching item with Id: {Id}", request.Id);

        var item = await _dbContext.Items
            .AsNoTracking()
            .Include(i => i.BaseUnit)
            .Include(i => i.ItemUnits).ThenInclude(u => u.Unit)
            .SingleOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        if (item is null)
        {
            _logger.LogWarning("Item with Id {Id} not found.", request.Id);
            return ItemErrors.NotFound;
        }

        var dto = item.ToDto();

        _logger.LogInformation("Item with Id {Id} retrieved successfully.", request.Id);

        return dto;
    }
}
