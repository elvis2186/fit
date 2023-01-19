using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

public class RefreshTokenManager : IRefreshTokenManager
{
    private ContextoServer _tokens;
    public RefreshTokenManager(ContextoServer tokens)
    {
        _tokens = tokens;
    }

    public async Task<RefreshToken> GetAsync(string Username)
    {
        var vaultItem = await _tokens.RefreshTokens.Where(t => t.UserName.Equals(Username)).FirstOrDefaultAsync();
        if (vaultItem is null)
        {
            return null;
        }
        try
        {
            var rToken = JsonConvert.DeserializeObject<RefreshToken>(vaultItem.RefreshTokenString);
            return rToken;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public async Task<bool> RemoveAsync(string Username)
    {
        var vaultItem = await _tokens.RefreshTokens.Where(t => t.UserName.Equals(Username)).FirstOrDefaultAsync();
        var transaction = await _tokens.Database.BeginTransactionAsync();
        _tokens.RefreshTokens.Remove(vaultItem);
        var changes = await _tokens.SaveChangesAsync();
        await transaction.CommitAsync();
        return changes > 0;
    }

    public async Task SetAsync(string Username, RefreshToken refreshToken)
    {
        var vaultItem = await _tokens.RefreshTokens.Where(t => t.UserName.Equals(refreshToken.Username)).FirstOrDefaultAsync();
        var transaction = await _tokens.Database.BeginTransactionAsync();

        if (vaultItem is null)
        {
            await _tokens.RefreshTokens.AddAsync(new TokenVaultItem
            {
                UserName = Username,
                RefreshTokenString = JsonConvert.SerializeObject(refreshToken)
            });
        }
        else
        {
            vaultItem.RefreshTokenString = JsonConvert.SerializeObject(refreshToken);
            _tokens.Entry(vaultItem).State = EntityState.Modified;
        }

        await _tokens.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}