using AlatrafClinic.Application.Features.WoundedCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.WoundedCards.Queries.GetWoundedCardByNumber;

public sealed record class GetWoundedCardByNumberQuery(string CardNumber) : IRequest<Result<WoundedCardDto>>;