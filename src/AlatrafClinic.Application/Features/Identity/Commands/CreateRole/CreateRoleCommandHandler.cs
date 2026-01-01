using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.CreateRole;

public sealed class CreateRoleCommandHandler
    : IRequestHandler<CreateRoleCommand, Result<string>>
{
    private readonly IIdentityService _identityService;

    public CreateRoleCommandHandler(IIdentityService identityService)
        => _identityService = identityService;

    public Task<Result<string>> Handle(
        CreateRoleCommand request,
        CancellationToken ct)
        => _identityService.CreateRoleAsync(request.Name, ct);
}
