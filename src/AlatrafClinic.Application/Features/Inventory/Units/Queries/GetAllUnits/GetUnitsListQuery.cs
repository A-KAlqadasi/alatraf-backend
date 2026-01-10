using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Units.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Units.Queries.GetUnitsListQuery;

public sealed record GetUnitsListQuery : IRequest<Result<List<UnitDto>>>;

