using foodBridgeAPI.Data;
using foodBridgeAPI.Models;
using foodBridgeAPI.DTOs.Usuario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace foodBridgeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsuariosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/usuarios
   [HttpGet]
public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetUsuarios()
{
    var usuarios = await _context.Usuarios
        .Select(u => new UsuarioDTO
        {
            Id = u.Id,
            Nombre = u.Nombre,
            TipoUsuario = u.TipoUsuario,
            Email = u.Email,
            Telefono = u.Telefono,
            Direccion = u.Direccion,
            FechaRegistro = u.FechaRegistro
        })
        .ToListAsync();

    return Ok(usuarios);
}

    // GET: api/usuarios/5
    [HttpGet("{id:int}")]
public async Task<ActionResult<UsuarioDTO>> GetUsuario(int id)
{
    var usuario = await _context.Usuarios.FindAsync(id);

    if (usuario == null)
    {
        return NotFound();
    }

    var dto = new UsuarioDTO
    {
        Id = usuario.Id,
        Nombre = usuario.Nombre,
        TipoUsuario = usuario.TipoUsuario,
        Email = usuario.Email,
        Telefono = usuario.Telefono,
        Direccion = usuario.Direccion,
        FechaRegistro = usuario.FechaRegistro
    };

    return Ok(dto);
}

    // POST: api/usuarios
   [HttpPost]
public async Task<ActionResult<UsuarioDTO>> CrearUsuario(CrearUsuarioDto dto)
{
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

    var respuesta = new UsuarioDTO
    {
        Id = usuario.Id,
        Nombre = usuario.Nombre,
        TipoUsuario = usuario.TipoUsuario,
        Email = usuario.Email,
        Telefono = usuario.Telefono,
        Direccion = usuario.Direccion,
        FechaRegistro = usuario.FechaRegistro
    };

    return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, respuesta);
}
}