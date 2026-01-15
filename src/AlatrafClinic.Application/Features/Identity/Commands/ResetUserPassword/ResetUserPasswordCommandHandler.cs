using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.ResetUserPassword;

public sealed class ResetUserPasswordCommandHandler
    : IRequestHandler<ResetUserPasswordCommand, Result<Updated>>
{
    private readonly IIdentityService _identityService;

    public ResetUserPasswordCommandHandler(IIdentityService identityService)
        => _identityService = identityService;

    public async Task<Result<Updated>> Handle(
        ResetUserPasswordCommand request,
        CancellationToken ct)
        => await _identityService.ResetUserPasswordAsync(
            request.UserId,
            request.NewPassword,
            ct);
}
