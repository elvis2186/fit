
using System.ComponentModel.DataAnnotations;

public class TokenVaultItem
{
    [Key]
    public string UserName { get; set; }
    public string RefreshTokenString { get; set; }
}