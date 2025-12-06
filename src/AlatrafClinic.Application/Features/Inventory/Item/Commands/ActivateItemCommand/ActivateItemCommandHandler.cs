
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Items;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Items.Commands.ActivateItemCommand;

public record ActivateItemCommandHandler : IRequestHandler<ActivateItemCommand, Result<ItemDto>>
{
    private readonly ILogger _logger;
    private readonly HybridCache _cache;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateItemCommandHandler(ILogger<ActivateItemCommandHandler> logger, HybridCache cache, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _cache = cache;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<ItemDto>> Handle(ActivateItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Items.GetByIdAsync(request.Id, cancellationToken);
        if (item == null)
            return ItemErrors.NotFound;

        var result = item.Activate();
        if (result.IsError)
            return result.Errors;


        await _unitOfWork.Items.UpdateAsync(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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