namespace foodBridgeAPI.DTOs.donacion;
using System.ComponentModel.DataAnnotations;
public class DonacionDto
{
  public int Id { get; set; }

    [Required(ErrorMessage = "El donante es obligatorio.")]
    public int DonanteId { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(150, ErrorMessage = "El título no puede superar los 150 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [StringLength(50, ErrorMessage = "La cantidad no puede superar los 50 caracteres.")]
    public string Cantidad { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
    public DateTime FechaVencimiento { get; set; }

    [StringLength(20)]
    public string Estado { get; set; } = string.Empty;

    // Datos generados por IA
    [Range(0, 100, ErrorMessage = "El score de urgencia debe estar entre 0 y 100.")]
    public int? ScoreUrgencia { get; set; }

    public bool? ContieneAlergenos { get; set; }

    public bool? RequiereCadenaFrio { get; set; }

    [StringLength(500)]
    public string? RecomendacionIa { get; set; }

    public DateTime FechaCreacion { get; set; }
}