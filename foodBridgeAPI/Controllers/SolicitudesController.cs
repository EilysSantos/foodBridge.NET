using foodBridgeAPI.Data;
using foodBridgeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using foodBridgeAPI.DTOs.solicitud;
namespace foodBridgeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolicitudesController : ControllerBase
{
    private readonly AppDbContext _context;

    public SolicitudesController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/solicitudes/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SolicitudDTO>> GetSolicitud(int id)
    {
        var solicitud = await _context.Solicitudes.FindAsync(id);

        if (solicitud is null)
        {
            return NotFound();
        }

        var solicitudDto = new SolicitudDTO
    {
        Id = solicitud.Id,
        DonacionId = solicitud.DonacionId,
        FundacionId = solicitud.FundacionId,
        EstadoSolicitud = solicitud.EstadoSolicitud,
        FechaSolicitud = solicitud.FechaSolicitud,
        FechaEntrega = solicitud.FechaEntrega
    };

        return Ok(solicitud);
    }

    // POST /api/solicitudes
    [HttpPost]
    public async Task<ActionResult<SolicitudDTO>> CrearSolicitud(CrearSolicitudDTO dto)
    {
         var donacion = await _context.Donaciones.FindAsync(dto.DonacionId);

    if (donacion is null)
    {
        return BadRequest("La donación indicada no existe.");
    }

    if (donacion.Estado != "Disponible")
    {
        return BadRequest($"La donación no está disponible (estado actual: {donacion.Estado}).");
    }

    var yaReservada = await _context.Solicitudes
        .AnyAsync(s => s.DonacionId == dto.DonacionId);

    if (yaReservada)
    {
        return BadRequest("Esta donación ya tiene una solicitud registrada.");
    }


    // Aquí conviertes DTO → Modelo
    var solicitud = new Solicitud
    {
        DonacionId = dto.DonacionId,
        FundacionId = dto.FundacionId,
        EstadoSolicitud = "Pendiente",
        FechaSolicitud = DateTime.UtcNow
    };


    _context.Solicitudes.Add(solicitud);

    donacion.Estado = "Reservado";

    await _context.SaveChangesAsync();


    // Modelo → DTO de respuesta
    var respuesta = new SolicitudDTO
    {
        Id = solicitud.Id,
        DonacionId = solicitud.DonacionId,
        FundacionId = solicitud.FundacionId,
        EstadoSolicitud = solicitud.EstadoSolicitud,
        FechaSolicitud = solicitud.FechaSolicitud,
        FechaEntrega = solicitud.FechaEntrega
    };


    return CreatedAtAction(
        nameof(GetSolicitud),
        new { id = solicitud.Id },
        respuesta
    );
    }
}
