namespace foodBridgeAPI.DTOs.donacion;

public class DonacionDto
{
    public int Id { get; set; }

    public int DonanteId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public string Cantidad { get; set; } = string.Empty;

    public DateTime FechaVencimiento { get; set; }

    public string Estado { get; set; } = string.Empty;

    public int? ScoreUrgencia { get; set; }

    public bool? ContieneAlergenos { get; set; }

    public bool? RequiereCadenaFrio { get; set; }

    public string? RecomendacionIa { get; set; }

    public DateTime FechaCreacion { get; set; }
}