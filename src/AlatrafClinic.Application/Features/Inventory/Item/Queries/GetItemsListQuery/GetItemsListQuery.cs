using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.GetItemsListQuery;

public sealed record GetItemsListQuery : IRequest<Result<List<ItemDto>>>;