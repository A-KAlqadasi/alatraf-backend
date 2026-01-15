using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Stores.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetStoreItemUnitsQuery;

public sealed record GetStoreItemUnitsQuery(int StoreId) : IRequest<Result<List<StoreItemUnitDto>>>;

