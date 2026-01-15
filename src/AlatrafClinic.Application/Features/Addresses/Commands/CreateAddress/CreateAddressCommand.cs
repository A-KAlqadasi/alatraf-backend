using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Addresses.Commands.CreateAddress;

public sealed record CreateAddressCommand(int Id, string Name) : IRequest<Result<Created>>;
