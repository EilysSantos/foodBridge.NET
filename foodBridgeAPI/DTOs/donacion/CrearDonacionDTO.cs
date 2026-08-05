namespace foodBridgeAPI.DTOs.donacion;

public class CrearDonacionDto
{
    public int DonanteId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public string Cantidad { get; set; } = string.Empty;

    public DateTime FechaVencimiento { get; set; }
}