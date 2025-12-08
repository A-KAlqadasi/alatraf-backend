using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetPatientTherapyCards;

public sealed record class GetPatientTherapyCardsQuery(int PatientId) : IRequest<Result<List<TherapyCardDiagnosisDto>>>;