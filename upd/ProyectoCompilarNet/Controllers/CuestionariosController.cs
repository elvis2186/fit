using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoCompilarNet.BussinesLogic.Services;

namespace ProyectoCompilarNet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CuestionariosController : ControllerBase
{
    private readonly ContextoServer _context;

    private readonly ILogger<CuestionariosController> _logger;

    private IOrden _orden;

    public CuestionariosController(ILogger<CuestionariosController> logger, ContextoServer context, IOrden orden)
    {
        _logger = logger;
        _context = context;
        _orden = orden;
    }

    [HttpGet("{llaveSegmento}")]
    [Authorize(Roles = "S")]
    public async Task<ActionResult> GetCuestionarioBySegmento(string llaveSegmento)
    {
        List<Cuestionarios>  cuestionarios = null;

        try
        {
            cuestionarios = await _orden.GetSegmentos(llaveSegmento);
        }
        catch(Exception e)
        {
            return BadRequest(e);
        }        

        return Ok(cuestionarios);
    }
}
