using System.ComponentModel.DataAnnotations;

namespace foodBridgeAPI.Models;

public class Donacion
{
    public int Id { get; set; }

    public int DonanteId { get; set; }
    public Usuario? Donante { get; set; }

    [Required, MaxLength(150)]
    public string Titulo { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    [MaxLength(50)]
    public string Cantidad { get; set; } = string.Empty;

    public DateTime FechaVencimiento { get; set; }

    [MaxLength(20)]
    public string Estado { get; set; } = "Disponible"; // Disponible | Reservado | Entregado | Expirado

    // Campos calculados por Google Gemini IA
    public int? ScoreUrgencia { get; set; }
    public bool? ContieneAlergenos { get; set; }
    public bool? RequiereCadenaFrio { get; set; }
    public string? RecomendacionIa { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public Solicitud? Solicitud { get; set; }
}
