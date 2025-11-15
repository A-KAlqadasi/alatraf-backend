using AlatrafClinic.Application.Features.IndustrialParts.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.IndustrialParts.Queries.GetIndustrialPartsForDropdown;

public sealed record GetIndustrialPartsForDropdownQuery : IRequest<Result<List<IndustrialPartDto>>>;