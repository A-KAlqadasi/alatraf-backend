using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Stores.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetStoreByIdQuery;

public sealed record GetStoreByIdQuery(int StoreId) : IRequest<Result<StoreDto>>;
