using AlatrafClinic.Application.Features.WoundedCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.WoundedCards.Commands.AddWoundedCard;

public sealed record class AddWoundedCardCommand(
    int PatientId, string CardNumber, DateOnly IssueDate, DateOnly ExpirationDate, string? CardImagePath = null
) : IRequest<Result<WoundedCardDto>>;