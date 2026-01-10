using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.GetInactiveItemsQuery;

public sealed record GetInactiveItemsQuery : IRequest<Result<List<ItemDto>>>;
