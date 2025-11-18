using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Patients.Queries.GetWoundedCardByNumber;

public sealed record class GetWoundedCardByNumberQuery(string CardNumber) : IRequest<Result<WoundedCardDto>>;