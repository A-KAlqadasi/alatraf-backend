using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateDeliveryTime;

public sealed record CreateDeliveryTimeCommand(int RepairCardId, DateTime DeliveryDate, string? Notes = null) : IRequest<Result<Created>>;