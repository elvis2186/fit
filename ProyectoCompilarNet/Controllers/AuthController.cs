using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProyectoCompilarNet.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{

    private readonly ILogger<AuthController> _logger;
    private readonly IJwtAuthManager _jwtAuthManager;
    private IConfiguration _conf;
    private readonly ContextoServer _context;

    public AuthController(ILogger<AuthController> logger,
    ContextoServer context,
    IJwtAuthManager jwtAuthManager,
    IConfiguration conf)
    {
        _logger = logger;
        _context = context;
        _jwtAuthManager = jwtAuthManager;
        _conf = conf;
    }

    [HttpPost(Name = "Auth")]
    public async Task<ActionResult> PostLogin(LoginUser usuario)
    {
        var result = await _context.Usuarios.Where(u => (u.UserName == usuario.UserName || u.Codigo == usuario.UserName)
                                     && (u.Password == usuario.Password)).FirstOrDefaultAsync();

        if (result == null)
        {
            return Unauthorized();
        }

        var claims = new[]{
                new Claim(ClaimTypes.Name, result.Codigo),
                new Claim(ClaimTypes.GivenName, result.UserName),
                new Claim(ClaimTypes.Role,result.Rol),
                new Claim(ClaimTypes.Country,result.Region)
        };

        var jwtResult = await _jwtAuthManager.GenerateTokens(result.Codigo, claims, DateTime.Now);
        return Ok(new
        {
            Id = result.Id,
            UserName = result.UserName,
            Codigo = result.Codigo,
            Rol = result.Rol,
            Region = result.Region,
            AccessToken = jwtResult.AccessToken,
            RefreshToken = jwtResult.RefreshToken
        });
    }
}
