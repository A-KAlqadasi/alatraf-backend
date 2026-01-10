
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Items.Commands.ActivateItemCommand;

public record ActivateItemCommandHandler : IRequestHandler<ActivateItemCommand, Result<ItemDto>>
{
    private readonly ILogger _logger;
    private readonly HybridCache _cache;
    private readonly IAppDbContext _dbContext;

    public ActivateItemCommandHandler(ILogger<ActivateItemCommandHandler> logger, HybridCache cache, IAppDbContext dbContext)
    {
        _logger = logger;
        _cache = cache;
        _dbContext = dbContext;
    }
    public async Task<Result<ItemDto>> Handle(ActivateItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _dbContext.Items.SingleOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
        if (item == null)
            return ItemErrors.NotFound;

        var result = item.Activate();
        if (result.IsError)
            return result.Errors;


        _dbContext.Items.Update(item);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            IsActive = item.IsActive,
            BaseUnitId = item.BaseUnitId,
            BaseUnitName = item.BaseUnit.Name
        };
    }
}