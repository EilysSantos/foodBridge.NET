using foodBridgeAPI.DTOs.solicitud;
using foodBridgeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace foodBridgeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolicitudesController : ControllerBase
{
    private readonly ISolicitudService _solicitudService;

    public SolicitudesController(ISolicitudService solicitudService)
    {
        _solicitudService = solicitudService;
    }

    // GET /api/solicitudes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SolicitudDTO>>> GetSolicitudes()
    {
        return Ok(await _solicitudService.GetSolicitudesAsync());
    }

    // GET /api/solicitudes/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SolicitudDTO>> GetSolicitud(int id)
    {
        var solicitud = await _solicitudService.GetSolicitudAsync(id);
        return solicitud is null ? NotFound() : Ok(solicitud);
    }

    // PUT /api/solicitudes/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<SolicitudDTO>> ActualizarSolicitud(int id, ActualizarSolicitudDTO dto)
    {
        var (solicitud, error, notFound) = await _solicitudService.ActualizarSolicitudAsync(id, dto);

        if (notFound)
        {
            return NotFound();
        }

        if (error is not null)
        {
            return BadRequest(error);
        }

        return Ok(solicitud);
    }

    // POST /api/solicitudes
    [HttpPost]
    public async Task<ActionResult<SolicitudDTO>> CrearSolicitud(CrearSolicitudDTO dto)
    {
        var (solicitud, error) = await _solicitudService.CrearSolicitudAsync(dto);

        if (error is not null)
        {
            return BadRequest(error);
        }

        return CreatedAtAction(
            nameof(GetSolicitud),
            new { id = solicitud!.Id },
            solicitud
        );
    }
}
