using AlatrafClinic.Application.Features.People.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Queries.GetPersonByNameOrNationalNoOrPhone;

public sealed record GetPersonByNameOrNationalNoOrPhoneQuery(
    string? Name = null,
    string? NationalNo = null,
    string? Phone = null
) : IRequest<Result<PersonDto>>;
