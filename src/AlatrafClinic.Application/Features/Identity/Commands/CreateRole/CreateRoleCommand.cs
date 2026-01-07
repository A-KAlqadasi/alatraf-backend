using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.CreateRole;

public sealed record CreateRoleCommand(
    string Name
) : IRequest<Result<string>>;
