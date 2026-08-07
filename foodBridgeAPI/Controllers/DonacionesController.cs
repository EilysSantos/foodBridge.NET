using foodBridgeAPI.Data;
using foodBridgeAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using foodBridgeAPI.DTOs.donacion;
using foodBridgeAPI.Services;

namespace foodBridgeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DonacionesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IGeminiService _geminiService;

    public DonacionesController(AppDbContext context, IGeminiService geminiService)
    {
        _context = context;
        _geminiService = geminiService;
    }

    // GET /api/donaciones
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DonacionDto>>> GetDonaciones()
    {
        var donaciones = await _context.Donaciones
            .OrderByDescending(d => d.ScoreUrgencia)
            .Select(d => new DonacionDto
            {
                Id = d.Id,
                DonanteId = d.DonanteId,
                Titulo = d.Titulo,
                Descripcion = d.Descripcion,
                Cantidad = d.Cantidad,
                FechaVencimiento = d.FechaVencimiento,
                Estado = d.Estado,
                ScoreUrgencia = d.ScoreUrgencia,
                ContieneAlergenos = d.ContieneAlergenos,
                RequiereCadenaFrio = d.RequiereCadenaFrio,
                RecomendacionIa = d.RecomendacionIa,
                FechaCreacion = d.FechaCreacion
            })
            .ToListAsync();

        return Ok(donaciones);
    }

    // GET /api/donaciones/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<IEnumerable<DonacionDto>>> Getdonacion(int Id)
    {
        var donacion = await _context.Donaciones.FindAsync(Id);

        if (donacion is null)
        {
            return NotFound();
        }
         var dto = new DonacionDto
        {
        Id = donacion.Id,
        DonanteId = donacion.DonanteId,
        Titulo = donacion.Titulo,
        Descripcion = donacion.Descripcion,
        Cantidad = donacion.Cantidad,
        FechaVencimiento = donacion.FechaVencimiento,
        Estado = donacion.Estado,
        ScoreUrgencia = donacion.ScoreUrgencia,
        ContieneAlergenos = donacion.ContieneAlergenos,
        RequiereCadenaFrio = donacion.RequiereCadenaFrio,
        RecomendacionIa = donacion.RecomendacionIa,
        FechaCreacion = donacion.FechaCreacion
       };

        return Ok(dto);
    }

    // POST /api/donaciones
    [HttpPost]
    public async Task<ActionResult<DonacionDto>> CrearDonacion(CrearDonacionDTO dto)
    {
        // 1. Evaluar con Gemini IA (JSON Estructurado)
        var evaluacionIa = await _geminiService.EvaluarDonacionAsync(
            dto.Titulo,
            dto.Descripcion,
            dto.Cantidad,
            dto.FechaVencimiento
        );

        var donacion = new Donacion
        {
            DonanteId = dto.DonanteId,
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            Cantidad = dto.Cantidad,
            FechaVencimiento = dto.FechaVencimiento,
            ScoreUrgencia = evaluacionIa?.ScoreUrgencia,
            ContieneAlergenos = evaluacionIa?.ContieneAlergenos,
            RequiereCadenaFrio = evaluacionIa?.RequiereCadenaFrio,
            RecomendacionIa = evaluacionIa?.RecomendacionIa
        };

        _context.Donaciones.Add(donacion);
        await _context.SaveChangesAsync();

        var respuesta = new DonacionDto
        {
            Id = donacion.Id,
            DonanteId = donacion.DonanteId,
            Titulo = donacion.Titulo,
            Descripcion = donacion.Descripcion,
            Cantidad = donacion.Cantidad,
            FechaVencimiento = donacion.FechaVencimiento,
            Estado = donacion.Estado,
            ScoreUrgencia = donacion.ScoreUrgencia,
            ContieneAlergenos = donacion.ContieneAlergenos,
            RequiereCadenaFrio = donacion.RequiereCadenaFrio,
            RecomendacionIa = donacion.RecomendacionIa,
            FechaCreacion = donacion.FechaCreacion
        };

        return CreatedAtAction(
            nameof(Getdonacion),
            new { id = donacion.Id },
            respuesta
        );
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

        return Ok(new DonacionDto
       {
        Id = donacion.Id,
        DonanteId = donacion.DonanteId,
        Titulo = donacion.Titulo,
        Descripcion = donacion.Descripcion,
        Cantidad = donacion.Cantidad,
        FechaVencimiento = donacion.FechaVencimiento,
        Estado = donacion.Estado,
        ScoreUrgencia = donacion.ScoreUrgencia,
        ContieneAlergenos = donacion.ContieneAlergenos,
        RequiereCadenaFrio = donacion.RequiereCadenaFrio,
        RecomendacionIa = donacion.RecomendacionIa,
        FechaCreacion = donacion.FechaCreacion
       });
    }
}
