using foodBridgeAPI.Data;
using foodBridgeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace foodBridgeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DonacionesController : ControllerBase
{
    private readonly AppDbContext _context;

    public DonacionesController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/donaciones
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Donacion>>> GetDonaciones()
    {
        var donaciones = await _context.Donaciones
            .OrderByDescending(d => d.ScoreUrgencia)
            .ToListAsync();

        return Ok(donaciones);
    }

    // GET /api/donaciones/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Donacion>> GetDonacion(int id)
    {
        var donacion = await _context.Donaciones.FindAsync(id);

        if (donacion is null)
        {
            return NotFound();
        }

        return Ok(donacion);
    }

    // POST /api/donaciones
    [HttpPost]
    public async Task<ActionResult<Donacion>> CrearDonacion(Donacion donacion)
    {
        _context.Donaciones.Add(donacion);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDonacion), new { id = donacion.Id }, donacion);
    }

    // PUT /api/donaciones/{id}/reservar
    [HttpPut("{id:int}/reservar")]
    public async Task<IActionResult> ReservarDonacion(int id)
    {
        var donacion = await _context.Donaciones.FindAsync(id);

        if (donacion is null)
        {
            return NotFound();
        }

        if (donacion.Estado != "Disponible")
        {
            return BadRequest($"La donación no está disponible (estado actual: {donacion.Estado}).");
        }

        donacion.Estado = "Reservado";
        await _context.SaveChangesAsync();

        return Ok(donacion);
    }
}
