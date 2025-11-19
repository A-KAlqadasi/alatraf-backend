using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Patients.Queries.GetDisabledCardByNumber;

public sealed record class GetDisabledCardByNumberQuery(string CardNumber) : IRequest<Result<DisabledCardDto>>;