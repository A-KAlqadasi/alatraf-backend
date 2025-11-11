using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.ChangeRepairCardStatus;

public sealed record ChangeRepairCardStatusCommand(int RepairCardId, RepairCardStatus NewStatus) : IRequest<Result<Updated>>;