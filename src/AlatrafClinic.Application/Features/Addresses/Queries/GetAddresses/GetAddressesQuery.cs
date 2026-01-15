using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Addresses.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Addresses.Queries.GetAddresses;

public sealed record GetAddressesQuery(int PageNumber, int PageSize, string? Search = null) : IRequest<Result<PaginatedList<AddressDto>>>;
