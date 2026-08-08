using foodBridgeAPI.DTOs.Usuario;
using foodBridgeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace foodBridgeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    // GET: api/usuarios
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetUsuarios()
    {
        return Ok(await _usuarioService.GetUsuariosAsync());
    }

    // GET: api/usuarios/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UsuarioDTO>> GetUsuario(int id)
    {
        var usuario = await _usuarioService.GetUsuarioAsync(id);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    // POST: api/usuarios
    [HttpPost]
    public async Task<ActionResult<UsuarioDTO>> CrearUsuario(CrearUsuarioDto dto)
    {
        var (usuario, error) = await _usuarioService.CrearUsuarioAsync(dto);
        if (error is not null)
        {
            return BadRequest(error);
        }

        return CreatedAtAction(nameof(GetUsuario), new { id = usuario!.Id }, usuario);
    }
}
