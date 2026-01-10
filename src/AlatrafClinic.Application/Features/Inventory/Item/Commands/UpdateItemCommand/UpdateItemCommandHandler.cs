using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Items.Commands.UpdateItemCommand;

public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, Result<ItemDto>>
{
    private readonly ILogger<UpdateItemCommandHandler> _logger;
    private readonly IAppDbContext _dbContext;
    private readonly HybridCache _cache;

    public UpdateItemCommandHandler(
        ILogger<UpdateItemCommandHandler> logger,
        IAppDbContext dbContext,
        HybridCache cache)
    {
        _logger = logger;
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<Result<ItemDto>> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _dbContext.Items.SingleOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
        if (item is null)
            return ItemErrors.NotFound;

        var updateResult = item.Update(request.Name, request.Description);
        if (updateResult.IsError)
            return updateResult.Errors;

        _dbContext.Items.Update(item);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Item (ID={Id}) updated successfully", item.Id);

        return new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            IsActive = item.IsActive,
            BaseUnitId = item.BaseUnitId,
            BaseUnitName = item.BaseUnit?.Name ?? "Unknown"
        };
    }
}
