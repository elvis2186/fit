using System.Security.Claims;

public interface IJwtAuthManager
{
    Task<JwtAuthResult> GenerateTokens(string username, Claim[] claims, DateTime hour);
    Task<JwtAuthResult> Refresh(string refreshToken, string accessToken, DateTime hour);
    Task RemoveRefreshTokenByUsername(string username);
}