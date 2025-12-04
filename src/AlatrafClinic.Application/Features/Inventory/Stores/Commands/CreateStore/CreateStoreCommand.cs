using AlatrafClinic.Application.Features.Inventory.Stores.Dtos;
using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.CreateStore;

public sealed record CreateStoreCommand(string Name) : IRequest<Result<StoreDto>>;
