using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Items.Commands.UpdateItemCommand;

public record UpdateItemCommand(
    int Id,
    string Name,
    string? Description
) : IRequest<Result<ItemDto>>;
