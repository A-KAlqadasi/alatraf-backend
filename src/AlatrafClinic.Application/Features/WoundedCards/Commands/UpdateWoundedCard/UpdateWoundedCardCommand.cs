using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.WoundedCards.Commands.UpdateWoundedCard;

public sealed record class UpdateWoundedCardCommand(
    int WoundedCardId, int PatientId, string CardNumber, DateOnly ExpirationDate, string? CardImagePath = null
) : IRequest<Result<Updated>>;