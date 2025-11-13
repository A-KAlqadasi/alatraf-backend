using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Units.Commands.DeleteUnitCommand;

public record DeleteUnitCommand(int Id) : IRequest<Result<Deleted>>;
