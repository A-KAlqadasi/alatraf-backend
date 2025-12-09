using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.InjurySides.Commands.DeleteInjurySide;

public sealed record DeleteInjurySideCommand(int InjurySideId) : IRequest<Result<Deleted>>;