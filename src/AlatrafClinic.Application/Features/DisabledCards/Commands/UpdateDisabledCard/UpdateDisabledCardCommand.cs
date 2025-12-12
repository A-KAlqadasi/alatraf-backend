using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.DisabledCards.Commands.UpdateDisabledCard;

public sealed record class UpdateDisabledCardCommand(
    int DisabledCardId, int PatientId, string CardNumber, DateOnly ExpirationDate, string? CardImagePath = null
) : IRequest<Result<Updated>>;