namespace foodBridgeAPI.DTOs.solicitud;

public class ActualizarSolicitudDTO
{
    public string EstadoSolicitud { get; set; } = string.Empty;

    public DateTime? FechaEntrega { get; set; }
}
