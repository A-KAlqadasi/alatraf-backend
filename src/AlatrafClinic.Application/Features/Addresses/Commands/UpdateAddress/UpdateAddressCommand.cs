using System.Security.Cryptography.X509Certificates;

using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Addresses.Commands.UpdateAddress;

public sealed record UpdateAddressCommand(
    int AddressId,
    string Name
    ) : IRequest<Result<Updated>>;
