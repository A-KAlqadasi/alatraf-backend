using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Patients.Queries.GetDisabledCards;

public sealed record GetDisabledCardsQuery : IRequest<Result<List<DisabledCardDto>>>;