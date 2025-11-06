using AlatrafClinic.Application.Features.Services.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Services.Queries.GetServiceById;

public sealed record GetServiceByIdQuery(int ServiceId) : IRequest<Result<ServiceDto>>;