using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.UpdateStore;

public sealed record UpdateStoreCommand(int StoreId, string Name) : IRequest<Result<Updated>>;
