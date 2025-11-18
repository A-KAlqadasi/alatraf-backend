using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Patients.Commands.AddDisabledCard;

public sealed record class AddDisabledCardCommand(
    int PatientId, string CardNumber, DateTime ExpirationDate, string? CardImagePath = null
) : IRequest<Result<DisabledCardDto>>;