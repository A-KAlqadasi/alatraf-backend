using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.IndustrialParts.Commands.UpdateIndustrialPart;

public sealed record UpdateIndustrialPartCommand(
    int IndustrialPartId,
    string Name,
    string? Description,
    List<UpdateIndustrialPartUnitCommand> Units
) : IRequest<Result<Updated>>;