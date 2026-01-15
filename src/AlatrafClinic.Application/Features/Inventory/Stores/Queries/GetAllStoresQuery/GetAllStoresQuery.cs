using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Stores.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetAllStoresQuery;

public sealed record GetAllStoresQuery() : IRequest<Result<List<StoreDto>>>;
