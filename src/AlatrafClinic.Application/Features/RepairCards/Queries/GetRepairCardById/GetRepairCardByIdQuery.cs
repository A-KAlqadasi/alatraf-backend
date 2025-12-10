using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetRepairCardById;

public sealed record GetRepairCardByIdQuery(int RepairCardId) : IRequest<Result<RepairCardDiagnosisDto>>;