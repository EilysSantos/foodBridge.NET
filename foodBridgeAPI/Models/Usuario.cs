using System.ComponentModel.DataAnnotations;

namespace foodBridgeAPI.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string TipoUsuario { get; set; } = string.Empty; // "Donante" | "Fundacion"

    [Required, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public ICollection<Donacion> Donaciones { get; set; } = new List<Donacion>();
    public ICollection<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();
}
