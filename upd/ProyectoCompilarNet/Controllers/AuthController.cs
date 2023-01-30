using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using ProyectoCompilarNet.BussinesLogic.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProyectoCompilarNet.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    [HttpPost("login")]
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

    [HttpGet]
    public async Task<ActionResult> Logout()
    {
        try
        {
            var user = User.Identity.Name;

            if(user == null)
            {
                return BadRequest("no es un token correcto");
            }
            await _jwtAuthManager.RemoveRefreshTokenByUsername(user);

            return Ok();

        }catch(Exception e)
        {
            Console.WriteLine(e);
            return NotFound();
        }        
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult> Refresh([FromBody] RefreshTokenRequest request) 
    {
        try
        {
            var user = User.Identity.Name;

            if (user == null)
            {
                return BadRequest("Request inválido");
            }

            var codigo = user;
            var userName = User.Claims
                .Where(c => c.Type == ClaimTypes.GivenName)
                .Select(c => c.Value)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return Unauthorized("token invalido");
            }

            var authHeader = Request.Headers[HeaderNames.Authorization];
            var accessToken = authHeader.Count > 0 ? authHeader[0].Replace("Bearer ", "") : null;
            var jwtResult = await _jwtAuthManager.Refresh(request.RefreshToken, accessToken, DateTime.Now);
            return Ok(new
            {
                UserName = userName,
                Codigo = codigo,
                Role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
                Region = User.FindFirst(ClaimTypes.Country)?.Value ?? string.Empty,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
            });
        }catch(SecurityTokenException e)
        {
          return Unauthorized(e.Message);
        }        
    }

}
