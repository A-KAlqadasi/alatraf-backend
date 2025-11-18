using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Patients.Commands.UpdateWoundedCard;

public sealed record class UpdateWoundedCardCommand(
    int WoundedCardId, int PatientId, string CardNumber, DateTime ExpirationDate, string? CardImagePath = null
) : IRequest<Result<Updated>>;