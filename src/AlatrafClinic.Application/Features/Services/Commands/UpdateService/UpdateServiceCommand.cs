using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Services.Commands.UpdateService;

public sealed record UpdateServiceCommand(int ServiceId, string Name, int? DepartmentId, decimal? Price = null) : IRequest<Result<Updated>>;