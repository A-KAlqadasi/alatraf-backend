using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.InjurySides.Commands.UpdateInjurySide;

public sealed record UpdateInjurySideCommand(int InjurySideId, string Name) : IRequest<Result<Updated>>;