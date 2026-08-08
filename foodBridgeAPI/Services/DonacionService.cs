using foodBridgeAPI.Data;
using foodBridgeAPI.DTOs.donacion;
using foodBridgeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace foodBridgeAPI.Services;

public class DonacionService : IDonacionService
{
    private readonly AppDbContext _context;
    private readonly IGroqService _groqService;

    public DonacionService(AppDbContext context, IGroqService groqService)
    {
        _context = context;
        _groqService = groqService;
    }

    public async Task<IEnumerable<DonacionDto>> GetDonacionesAsync()
    {
        return await _context.Donaciones
            .OrderByDescending(d => d.ScoreUrgencia)
            .Select(d => MapToDto(d))
            .ToListAsync();
    }

    public async Task<DonacionDto?> GetDonacionAsync(int id)
    {
        var donacion = await _context.Donaciones.FindAsync(id);
        return donacion is null ? null : MapToDto(donacion);
    }

    public async Task<DonacionDto> CrearDonacionAsync(CrearDonacionDTO dto)
    {
        var evaluacionIa = await _groqService.EvaluarDonacionAsync(
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

        return MapToDto(donacion);
    }

    public async Task<(DonacionDto? Donacion, string? Error)> ReservarDonacionAsync(int id)
    {
        var donacion = await _context.Donaciones.FindAsync(id);
        if (donacion is null)
        {
            return (null, null);
        }

        if (donacion.Estado != "Disponible")
        {
            return (null, $"La donación no está disponible (estado actual: {donacion.Estado}).");
        }

        donacion.Estado = "Reservado";
        await _context.SaveChangesAsync();

        return (MapToDto(donacion), null);
    }

    private static DonacionDto MapToDto(Donacion donacion) => new()
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
}
