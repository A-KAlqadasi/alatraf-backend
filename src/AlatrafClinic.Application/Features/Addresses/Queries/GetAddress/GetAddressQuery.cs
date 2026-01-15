using AlatrafClinic.Application.Features.Addresses.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Addresses.Queries.GetAddress;

public sealed record GetAddressQuery(int Id) : IRequest<Result<AddressDto>>;
