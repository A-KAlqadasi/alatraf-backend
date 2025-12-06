using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Inventory.Stores.Dtos;
using AlatrafClinic.Application.Features.Inventory.Stores.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Inventory.Stores;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.CreateStore;

public class CreateStoreCommandHandler : IRequestHandler<CreateStoreCommand, Result<StoreDto>>
{
    private readonly ILogger<CreateStoreCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStoreCommandHandler(ILogger<CreateStoreCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<StoreDto>> Handle(CreateStoreCommand command, CancellationToken ct)
    {
        var createResult = Store.Create(command.Name);
        if (createResult.IsError)
        {
            _logger.LogWarning("Failed to create store: {Errors}", string.Join(',', createResult.Errors));
            return createResult.Errors;
        }

        var store = createResult.Value;
        await _unitOfWork.Stores.AddAsync(store, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Created store {StoreId}", store.Id);
        return store.ToDto();
    }
}