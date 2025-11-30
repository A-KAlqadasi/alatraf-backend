using AlatrafClinic.Application.Features.DisabledCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.DisabledCards.Queries.GetDisabledCardByNumber;

public sealed record class GetDisabledCardByNumberQuery(string CardNumber) : IRequest<Result<DisabledCardDto>>;