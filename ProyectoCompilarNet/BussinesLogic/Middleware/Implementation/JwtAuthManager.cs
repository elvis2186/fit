
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

public class JwtAuthManager : IJwtAuthManager
{
    private ILogger<JwtAuthManager> _logger;
    private IRefreshTokenManager _usersRefreshTokens;
    private readonly JwtTokenConfiguration _jwtTokenConfig;
    private readonly byte[] _secret;
    public JwtAuthManager(JwtTokenConfiguration jwtToken,
    IMemoryCache memoryCache,
    IRefreshTokenManager refreshTokenManager,
    ILogger<JwtAuthManager> logger)
    {
        _jwtTokenConfig = jwtToken;
        _logger = logger;
        _usersRefreshTokens = refreshTokenManager;
        _secret = System.Text.Encoding.ASCII.GetBytes(jwtToken.Secret);
    }

    public async Task<JwtAuthResult> GenerateTokens(string username, Claim[] claims, DateTime hour)
    {
        var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);
        var jwtToken = new JwtSecurityToken(
                _jwtTokenConfig.Issuer,
                shouldAddAudienceClaim ? _jwtTokenConfig.Audience : string.Empty,
                claims,
                expires: hour.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        var refreshToken = new RefreshToken
        {
            Username = username,
            TokenString = GenerateRefreshTokenString(),
            ExpireAt = hour.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration)
        };
        await _usersRefreshTokens.SetAsync(refreshToken.Username, refreshToken);

        return new JwtAuthResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<JwtAuthResult> Refresh(string refreshToken, string accessToken, DateTime hour)
    {
        var (principal, jwtToken) = DecodeJwtToken(accessToken);
        if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
        {
            throw new SecurityTokenException("Invalid jwt token");
        }

        var userName = principal.Identity.Name;
        RefreshToken existingRefreshToken = await _usersRefreshTokens.GetAsync(userName);
        if (existingRefreshToken is null)
        {
            throw new SecurityTokenException("Usuario no firmado para Refresh");
        }
        if (existingRefreshToken.TokenString != refreshToken || existingRefreshToken.ExpireAt < hour)
        {
            throw new SecurityTokenException("Refresh token expirado");
        }

        return await GenerateTokens(userName, principal.Claims.ToArray(), hour);
    }

    public async Task RemoveRefreshTokenByUsername(string username)
    {
        var removed = await _usersRefreshTokens.RemoveAsync(username);
        _logger.LogInformation($"usuario {username} cerro session {DateTime.Now}");
    }

    private static string GenerateRefreshTokenString()
    {
        var randomNumber = new byte[32];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new SecurityTokenException("Invalid token");
        }
        var principal = new JwtSecurityTokenHandler()
            .ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _jwtTokenConfig.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_secret),
                    ValidAudience = _jwtTokenConfig.Audience,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.FromMinutes(1)
                },
                out var validatedToken);
        return (principal, validatedToken as JwtSecurityToken);
    }
}