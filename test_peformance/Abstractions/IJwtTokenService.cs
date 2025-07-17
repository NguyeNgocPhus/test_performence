using System.Security.Claims;

namespace test_peformance.Abstractions;

public interface IJwtTokenService
{
    public ClaimsPrincipal DecodeAccessToken(string token);

    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}