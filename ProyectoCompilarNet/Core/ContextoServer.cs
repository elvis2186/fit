using Microsoft.EntityFrameworkCore;

public class ContextoServer : DbContext
{
    public ContextoServer()
    {

    }

    public ContextoServer(DbContextOptions<ContextoServer> options) : base(options)
    {

    }

    public virtual DbSet<TokenVaultItem> RefreshTokens { get; set; }
    public virtual DbSet<Usuarios> Usuarios { get; set; }
    public virtual DbSet<Cuestionarios> Cuestionarios { get; set; }

}