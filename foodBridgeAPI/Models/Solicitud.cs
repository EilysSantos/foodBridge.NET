using System.ComponentModel.DataAnnotations;

namespace foodBridgeAPI.Models;

public class Solicitud
{
    public int Id { get; set; }

    public int DonacionId { get; set; } // UNIQUE (1:1 con Donacion)
    public Donacion? Donacion { get; set; }

    public int FundacionId { get; set; }
    public Usuario? Fundacion { get; set; }

    [MaxLength(20)]
    public string EstadoSolicitud { get; set; } = "Pendiente"; // Pendiente | Completada | Cancelada

    public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;

    public DateTime? FechaEntrega { get; set; }
}
