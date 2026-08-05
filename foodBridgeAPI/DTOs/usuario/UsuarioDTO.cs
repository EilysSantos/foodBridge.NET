namespace foodBridgeAPI.DTOs.Usuario;

public class UsuarioDTO
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string TipoUsuario { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public DateTime FechaRegistro { get; set; }
}