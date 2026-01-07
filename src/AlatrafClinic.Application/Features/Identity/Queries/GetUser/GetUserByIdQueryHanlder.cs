using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Identity.Queries.GetUser;

public class GetUserByIdQueryHanlder(ILogger<GetUserByIdQueryHanlder> logger, IIdentityService identityService)
    : IRequestHandler<GetUserByIdQuery, Result<UserDetailsDto>>
{
    private readonly ILogger<GetUserByIdQueryHanlder> _logger = logger;
    private readonly IIdentityService _identityService = identityService;
    public async Task<Result<UserDetailsDto>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var getUserByIdResult = await _identityService.GetUserByIdAsync(request.UserId!);

        if (getUserByIdResult.IsError)
        {
            _logger.LogError("User with Id { UserId }{ErrorDetails}", request.UserId, getUserByIdResult.TopError.Description);

            return getUserByIdResult.Errors;
        }

        return getUserByIdResult.Value;
    }
}