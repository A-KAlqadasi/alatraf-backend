using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.WoundedCards.Commands.DeleteWoundedCard;

public sealed record class DeleteWoundedCardCommand(int WoundedCardId) : IRequest<Result<Deleted>>;
