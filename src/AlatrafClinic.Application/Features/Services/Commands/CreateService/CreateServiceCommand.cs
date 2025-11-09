using AlatrafClinic.Application.Features.Services.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Services.Commands.CreateService;

public sealed record CreateServiceCommand(string Name, int DepartmentId, decimal? Price = null) : IRequest<Result<ServiceDto>>;