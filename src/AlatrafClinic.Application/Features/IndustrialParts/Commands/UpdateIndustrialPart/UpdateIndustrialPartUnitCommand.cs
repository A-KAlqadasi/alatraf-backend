using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.IndustrialParts.Commands.UpdateIndustrialPart;

public sealed record UpdateIndustrialPartUnitCommand(
    int UnitId,
    decimal Price
) : IRequest<Result<Success>>;