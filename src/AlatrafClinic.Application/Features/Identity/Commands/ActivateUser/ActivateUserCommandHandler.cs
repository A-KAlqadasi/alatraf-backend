using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.ActivateUser;

public sealed class ActivateUserCommandHandler
    : IRequestHandler<ActivateUserCommand, Result<Updated>>
{
    private readonly IIdentityService _identityService;

    public ActivateUserCommandHandler(IIdentityService identityService)
        => _identityService = identityService;

    public async Task<Result<Updated>> Handle(
        ActivateUserCommand request,
        CancellationToken ct)
    {
        return await _identityService
            .ActivateUserAsync(request.UserId, request.IsActive, ct);
    }
}
