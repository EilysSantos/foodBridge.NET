using System.ComponentModel.DataAnnotations;

namespace foodBridgeAPI.DTOs.solicitud;

public class ActualizarSolicitudDTO
{
    [Required(ErrorMessage = "El estado de la solicitud es obligatorio.")]
    [StringLength(20, ErrorMessage = "El estado no puede superar los 20 caracteres.")]
    public string EstadoSolicitud { get; set; } = string.Empty;

    public DateTime? FechaEntrega { get; set; }
}
