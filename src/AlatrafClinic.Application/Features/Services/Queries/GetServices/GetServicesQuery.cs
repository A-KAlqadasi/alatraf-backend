using AlatrafClinic.Application.Features.Services.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Services.Queries.GetServices;

public record GetServicesQuery : IRequest<Result<List<ServiceDto>>>;