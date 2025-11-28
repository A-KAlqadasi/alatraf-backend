using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Queries.RefreshTokens;

public record RefreshTokenQuery(string RefreshToken, string ExpiredAccessToken) : IRequest<Result<TokenResponse>>;