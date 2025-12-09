using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.InjuryReasons.Commands.DeleteInjuryReason;

public sealed record DeleteInjuryReasonCommand(int InjuryReasonId) : IRequest<Result<Deleted>>;