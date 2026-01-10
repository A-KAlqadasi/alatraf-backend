using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.GetItemsWithUnitsQuery;

public sealed record GetItemsWithUnitsQuery : IRequest<Result<List<ItemDto>>>;
