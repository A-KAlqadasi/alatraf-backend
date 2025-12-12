using AlatrafClinic.Application.Features.DisabledCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.DisabledCards.Commands.AddDisabledCard;

public sealed record class AddDisabledCardCommand(
    int PatientId, string CardNumber, DateOnly ExpirationDate, string? CardImagePath = null
) : IRequest<Result<DisabledCardDto>>;