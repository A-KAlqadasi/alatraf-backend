using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.UpdateOrderSection;

public sealed record UpdateOrderSectionCommand(int RepairCardId, int OrderId, int SectionId) : IRequest<Result<Updated>>;
