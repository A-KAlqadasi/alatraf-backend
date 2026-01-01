using AlatrafClinic.Application.Features.IndustrialParts.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.IndustrialParts.Queries.GetIndustrialPartById;

public sealed record GetIndustrialPartByIdQuery(int IndustrialPartId) : IRequest<Result<IndustrialPartDto>>;