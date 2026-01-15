using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Items.Queries.GetItemByIdQuery;

public sealed record GetItemByIdQuery(int Id) : IRequest<Result<ItemDto>>;