using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.InjuryReasons.Commands.UpdateInjuryReason;

public sealed record UpdateInjuryReasonCommand(int InjuryReasonId, string Name) : IRequest<Result<Updated>>;