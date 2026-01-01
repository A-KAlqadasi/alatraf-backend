using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.ChangeUserCredentials;

public sealed class ChangeUserCredentialsCommandHandler
    : IRequestHandler<ChangeUserCredentialsCommand, Result<Updated>>
{
    private readonly IIdentityService _identityService;

    public ChangeUserCredentialsCommandHandler(IIdentityService identityService)
        => _identityService = identityService;

    public Task<Result<Updated>> Handle(
        ChangeUserCredentialsCommand request,
        CancellationToken ct)
        => _identityService.ChangeUserCredentialsAsync(
            request.UserId,
            request.OldPassword,
            request.NewPassword,
            request.NewUsername,
            ct);
}
