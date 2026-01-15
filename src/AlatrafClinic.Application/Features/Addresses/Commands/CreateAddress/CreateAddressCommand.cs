using AlatrafClinic.Application.Features.Addresses.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Addresses.Commands.CreateAddress;

public sealed record CreateAddressCommand(string Name) : IRequest<Result<AddressDto>>;
