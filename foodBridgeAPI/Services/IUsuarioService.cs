using foodBridgeAPI.DTOs.Usuario;

namespace foodBridgeAPI.Services;

public interface IUsuarioService
{
    Task<IEnumerable<UsuarioDTO>> GetUsuariosAsync();
    Task<UsuarioDTO?> GetUsuarioAsync(int id);
    Task<(UsuarioDTO? Usuario, string? Error)> CrearUsuarioAsync(CrearUsuarioDto dto);
}
