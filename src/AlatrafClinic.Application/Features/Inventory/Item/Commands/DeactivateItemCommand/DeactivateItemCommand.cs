using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;
namespace AlatrafClinic.Application.Features.Inventory.Items.Commands.DeactivateItemCommand;
public sealed record DeactivateItemCommand(int Id) : IRequest<Result<ItemDto>>;
