using foodBridgeAPI.Data;
using foodBridgeAPI.DTOs.Usuario;
using foodBridgeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace foodBridgeAPI.Services;

public class UsuarioService : IUsuarioService
{
    private readonly AppDbContext _context;

    public UsuarioService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UsuarioDTO>> GetUsuariosAsync()
    {
        return await _context.Usuarios
            .Select(u => MapToDto(u))
            .ToListAsync();
    }

    public async Task<UsuarioDTO?> GetUsuarioAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        return usuario is null ? null : MapToDto(usuario);
    }

    public async Task<(UsuarioDTO? Usuario, string? Error)> CrearUsuarioAsync(CrearUsuarioDto dto)
    {
        var emailEnUso = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email);
        if (emailEnUso)
        {
            return (null, "Ya existe un usuario registrado con ese correo electrónico.");
        }

        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            TipoUsuario = dto.TipoUsuario,
            Email = dto.Email,
            Telefono = dto.Telefono,
            Direccion = dto.Direccion
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return (MapToDto(usuario), null);
    }

    private static UsuarioDTO MapToDto(Usuario usuario) => new()
    {
        Id = usuario.Id,
        Nombre = usuario.Nombre,
        TipoUsuario = usuario.TipoUsuario,
        Email = usuario.Email,
        Telefono = usuario.Telefono,
        Direccion = usuario.Direccion,
        FechaRegistro = usuario.FechaRegistro
    };
}
