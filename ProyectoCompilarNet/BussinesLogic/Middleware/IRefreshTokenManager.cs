
public interface IRefreshTokenManager
{
    Task SetAsync(string Username, RefreshToken refreshToken);
    Task<RefreshToken> GetAsync(string Username);
    Task<bool> RemoveAsync(string Username);
}