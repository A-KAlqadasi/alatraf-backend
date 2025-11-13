using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Items.Commands.ActivateItemCommand;

public record ActivateItemCommand(int Id) : IRequest<Result<ItemDto>>;