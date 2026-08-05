using foodBridgeAPI.Data;
using foodBridgeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<ActionResult<Solicitud>> GetSolicitud(int id)
    {
        var solicitud = await _context.Solicitudes.FindAsync(id);

        if (solicitud is null)
        {
            return NotFound();
        }

        return Ok(solicitud);
    }

    // POST /api/solicitudes
    [HttpPost]
    public async Task<ActionResult<Solicitud>> CrearSolicitud(Solicitud solicitud)
    {
        var donacion = await _context.Donaciones.FindAsync(solicitud.DonacionId);

        if (donacion is null)
        {
            return BadRequest("La donación indicada no existe.");
        }

        if (donacion.Estado != "Disponible")
        {
            return BadRequest($"La donación no está disponible (estado actual: {donacion.Estado}).");
        }

        var yaReservada = await _context.Solicitudes.AnyAsync(s => s.DonacionId == solicitud.DonacionId);
        if (yaReservada)
        {
            return BadRequest("Esta donación ya tiene una solicitud registrada.");
        }

        _context.Solicitudes.Add(solicitud);
        donacion.Estado = "Reservado";

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSolicitud), new { id = solicitud.Id }, solicitud);
    }
}
