using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.DeleteStore;

public sealed record DeleteStoreCommand(int StoreId) : IRequest<Result<Deleted>>;
