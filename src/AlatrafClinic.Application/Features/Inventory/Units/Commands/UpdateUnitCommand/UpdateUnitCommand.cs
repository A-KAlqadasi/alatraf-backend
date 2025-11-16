using AlatrafClinic.Application.Features.Inventory.Units.Dtos;
using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Units.Commands.UpdateUnitCommand;

public record UpdateUnitCommand(int Id, string Name) : IRequest<Result<UnitDto>>;
