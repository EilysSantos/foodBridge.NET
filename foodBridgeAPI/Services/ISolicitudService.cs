using foodBridgeAPI.DTOs.solicitud;

namespace foodBridgeAPI.Services;

public interface ISolicitudService
{
    Task<IEnumerable<SolicitudDTO>> GetSolicitudesAsync();
    Task<SolicitudDTO?> GetSolicitudAsync(int id);
    Task<(SolicitudDTO? Solicitud, string? Error)> CrearSolicitudAsync(CrearSolicitudDTO dto);
    Task<(SolicitudDTO? Solicitud, string? Error, bool NotFound)> ActualizarSolicitudAsync(int id, ActualizarSolicitudDTO dto);
}
