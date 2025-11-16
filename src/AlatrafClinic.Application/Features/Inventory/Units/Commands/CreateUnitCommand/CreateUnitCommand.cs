using AlatrafClinic.Application.Features.Inventory.Units.Dtos;
using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Units.Commands.CreateUnitCommand;

public record CreateUnitCommand(string Name) : IRequest<Result<UnitDto>>;
