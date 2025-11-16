using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.IndustrialParts.Commands.CreateIndustrialPart;

public sealed record CreateIndustrialPartUnitCommand(
    int UnitId,
    decimal Price
) : IRequest<Result<Success>>;