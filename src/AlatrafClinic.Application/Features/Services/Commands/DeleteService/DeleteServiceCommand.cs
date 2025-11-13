using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Services.Commands.DeleteService;

public sealed record DeleteServiceCommand(int ServiceId) : IRequest<Result<Deleted>>;