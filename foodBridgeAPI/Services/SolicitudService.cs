using foodBridgeAPI.Data;
using foodBridgeAPI.DTOs.solicitud;
using foodBridgeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace foodBridgeAPI.Services;

public class SolicitudService : ISolicitudService
{
    private static readonly string[] EstadosValidos = { "Pendiente", "Completada", "Cancelada" };

    private readonly AppDbContext _context;

    public SolicitudService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SolicitudDTO>> GetSolicitudesAsync()
    {
        return await _context.Solicitudes
            .Select(s => MapToDto(s))
            .ToListAsync();
    }

    public async Task<SolicitudDTO?> GetSolicitudAsync(int id)
    {
        var solicitud = await _context.Solicitudes.FindAsync(id);
        return solicitud is null ? null : MapToDto(solicitud);
    }

    public async Task<(SolicitudDTO? Solicitud, string? Error)> CrearSolicitudAsync(CrearSolicitudDTO dto)
    {
        var donacion = await _context.Donaciones.FindAsync(dto.DonacionId);
        if (donacion is null)
        {
            return (null, "La donación indicada no existe.");
        }

        if (donacion.Estado != "Disponible")
        {
            return (null, $"La donación no está disponible (estado actual: {donacion.Estado}).");
        }

        var yaReservada = await _context.Solicitudes.AnyAsync(s => s.DonacionId == dto.DonacionId);
        if (yaReservada)
        {
            return (null, "Esta donación ya tiene una solicitud registrada.");
        }

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

        return (MapToDto(solicitud), null);
    }

    public async Task<(SolicitudDTO? Solicitud, string? Error, bool NotFound)> ActualizarSolicitudAsync(int id, ActualizarSolicitudDTO dto)
    {
        var solicitud = await _context.Solicitudes.FindAsync(id);
        if (solicitud is null)
        {
            return (null, null, true);
        }

        if (!EstadosValidos.Contains(dto.EstadoSolicitud))
        {
            return (null, $"Estado inválido. Los valores permitidos son: {string.Join(", ", EstadosValidos)}.", false);
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

        return (MapToDto(solicitud), null, false);
    }

    private static SolicitudDTO MapToDto(Solicitud solicitud) => new()
    {
        Id = solicitud.Id,
        DonacionId = solicitud.DonacionId,
        FundacionId = solicitud.FundacionId,
        EstadoSolicitud = solicitud.EstadoSolicitud,
        FechaSolicitud = solicitud.FechaSolicitud,
        FechaEntrega = solicitud.FechaEntrega
    };
}
