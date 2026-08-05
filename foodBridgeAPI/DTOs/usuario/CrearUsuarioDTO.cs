using System.ComponentModel.DataAnnotations;

namespace foodBridgeAPI.DTOs.Usuario;

public class CrearUsuarioDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de usuario es obligatorio.")]
    [StringLength(20, ErrorMessage = "El tipo de usuario no puede superar los 20 caracteres.")]
    public string TipoUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "El número de teléfono no es válido.")]
    [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
    public string? Telefono { get; set; }

    [StringLength(250, ErrorMessage = "La dirección no puede superar los 250 caracteres.")]
    public string? Direccion { get; set; }
}