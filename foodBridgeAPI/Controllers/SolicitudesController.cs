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

    // GET /api/solicitudes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SolicitudDTO>>> GetSolicitudes()
    {
        var solicitudes = await _context.Solicitudes
            .Select(s => new SolicitudDTO
            {
                Id = s.Id,
                DonacionId = s.DonacionId,
                FundacionId = s.FundacionId,
                EstadoSolicitud = s.EstadoSolicitud,
                FechaSolicitud = s.FechaSolicitud,
                FechaEntrega = s.FechaEntrega
            })
            .ToListAsync();

        return Ok(solicitudes);
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

        return Ok(solicitudDto);
    }

    // PUT /api/solicitudes/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<SolicitudDTO>> ActualizarSolicitud(int id, ActualizarSolicitudDTO dto)
    {
        var solicitud = await _context.Solicitudes.FindAsync(id);

        if (solicitud is null)
        {
            return NotFound();
        }

        var estadosValidos = new[] { "Pendiente", "Completada", "Cancelada" };
        if (!estadosValidos.Contains(dto.EstadoSolicitud))
        {
            return BadRequest($"Estado inválido. Los valores permitidos son: {string.Join(", ", estadosValidos)}.");
        }

        solicitud.EstadoSolicitud = dto.EstadoSolicitud;
        solicitud.FechaEntrega = dto.FechaEntrega;

        if (dto.EstadoSolicitud == "Completada")
        {
            var donacion = await _context.Donaciones.FindAsync(solicitud.DonacionId);
            if (donacion is not null)
            {
                donacion.Estado = "Entregado";
            }
        }

        await _context.SaveChangesAsync();

        var respuesta = new SolicitudDTO
        {
            Id = solicitud.Id,
            DonacionId = solicitud.DonacionId,
            FundacionId = solicitud.FundacionId,
            EstadoSolicitud = solicitud.EstadoSolicitud,
            FechaSolicitud = solicitud.FechaSolicitud,
            FechaEntrega = solicitud.FechaEntrega
        };

        return Ok(respuesta);
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
