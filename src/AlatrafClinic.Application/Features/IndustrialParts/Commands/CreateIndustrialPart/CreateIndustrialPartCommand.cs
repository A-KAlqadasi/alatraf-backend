using AlatrafClinic.Application.Features.IndustrialParts.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.IndustrialParts.Commands.CreateIndustrialPart;

public sealed record CreateIndustrialPartCommand(
    string Name,
    string? Description,
    List<CreateIndustrialPartUnitCommand> Units
) : IRequest<Result<IndustrialPartDto>>;