using AlatrafClinic.Domain.Common.Results;

using MediatR;
namespace AlatrafClinic.Application.Features.Inventory.Items.Commands.RemoveItemUnitCommand;
public sealed record RemoveItemUnitCommand(int ItemId, int UnitId) : IRequest<Result<Updated>>;
