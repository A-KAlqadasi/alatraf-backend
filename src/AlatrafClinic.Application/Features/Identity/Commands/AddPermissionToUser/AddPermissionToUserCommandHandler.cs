using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Identity.Commands.AddPermissionToUser;

public sealed class AddPermissionToUserCommandHandler
    : IRequestHandler<AddPermissionToUserCommand, Result<bool>>
{
    private readonly ILogger<AddPermissionToUserCommandHandler> _logger;
    private readonly IIdentityService _identityService;

    public AddPermissionToUserCommandHandler(
        ILogger<AddPermissionToUserCommandHandler> logger,
        IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
    }

    public async Task<Result<bool>> Handle(AddPermissionToUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.AddPermissionToUserAsync(
            request.UserId,
            request.PermissionName,
            cancellationToken);

        if (result.IsError)
        {
            _logger.LogError(
                "Error adding permission '{Permission}' to user '{UserId}': {ErrorDescription}",
                request.PermissionName,
                request.UserId,
                result.TopError.Description);

            return result.Errors;
        }

        return true;
    }
}