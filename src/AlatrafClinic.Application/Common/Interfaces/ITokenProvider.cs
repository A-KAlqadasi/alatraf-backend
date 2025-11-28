using System.Security.Claims;

using AlatrafClinic.Application.Features.Identity;
using AlatrafClinic.Application.Features.Identity.Dtos;

using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Common.Interfaces;

public interface ITokenProvider
{
    Task<Result<TokenResponse>> GenerateJwtTokenAsync(AppUserDto user, CancellationToken ct = default);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}