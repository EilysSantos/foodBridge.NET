namespace foodBridgeAPI.DTOs.solicitud;

public class SolicitudDTO
{
    public int Id { get; set; }

    public int DonacionId { get; set; }

    public int FundacionId { get; set; }

    public string EstadoSolicitud { get; set; } = string.Empty;

    public DateTime FechaSolicitud { get; set; }

    public DateTime? FechaEntrega { get; set; }
}