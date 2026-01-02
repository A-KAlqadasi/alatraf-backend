using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Commands.CreateUser;

public sealed class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, Result<string>>
{
    private readonly IIdentityService _identityService;

    public CreateUserCommandHandler(IIdentityService identityService)
        => _identityService = identityService;

    public Task<Result<string>> Handle(
        CreateUserCommand request,
        CancellationToken ct)
        => _identityService.CreateUserAsync(
            request.PersonId,
            request.UserName,
            request.Password,
            request.IsActive,
            ct);
}
