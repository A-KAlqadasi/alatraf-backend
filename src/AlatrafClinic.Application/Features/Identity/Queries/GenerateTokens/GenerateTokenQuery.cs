using AlatrafClinic.Domain.Common.Results;


using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Queries.GenerateTokens;

public record LoginRequest(
    string UserName,
    string Password) : IRequest<Result<TokenResponse>>;