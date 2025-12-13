using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetPatientRepairCards;

public sealed record class GetPatientRepairCardsQuery(int PatientId) : IRequest<Result<List<RepairCardDiagnosisDto>>>;