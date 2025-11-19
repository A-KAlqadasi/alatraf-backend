using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Patients.Commands.AddWoundedCard;

public sealed record class AddWoundedCardCommand(
    int PatientId, string CardNumber, DateTime ExpirationDate, string? CardImagePath = null
) : IRequest<Result<WoundedCardDto>>;