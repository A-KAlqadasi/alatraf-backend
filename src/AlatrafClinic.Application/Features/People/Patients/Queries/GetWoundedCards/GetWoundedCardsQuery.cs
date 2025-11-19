using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Patients.Queries.GetWoundedCards;

public sealed record GetWoundedCardsQuery : IRequest<Result<List<WoundedCardDto>>>;