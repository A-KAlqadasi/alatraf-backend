using AlatrafClinic.Application.Features.Inventory.Units.Dtos;
using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Units.Queries.GetUnitByIdQuery;

public record GetUnitByIdQuery(int Id) : IRequest<Result<UnitDto>>;
