using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoCompilarNet.BussinesLogic.Models;
using Microsoft.EntityFrameworkCore;

namespace ProyectoCompilarNet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CuestionariosController : ControllerBase
{
    private readonly ContextoServer _context;

    private readonly ILogger<CuestionariosController> _logger;

    public CuestionariosController(ILogger<CuestionariosController> logger, ContextoServer context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("{llaveSegmento}")]
    [Authorize(Roles = "S")]
    public async Task<ActionResult> GetCuestionarioBySegmento(string llaveSegmento)
    {
        var cuestionarios = await _context.Cuestionarios.Where(c => c.Llave.StartsWith(llaveSegmento)).ToListAsync();
        return Ok(cuestionarios);
    }
}
