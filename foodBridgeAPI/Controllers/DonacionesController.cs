using foodBridgeAPI.DTOs.donacion;
using foodBridgeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace foodBridgeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DonacionesController : ControllerBase
{
    private readonly IDonacionService _donacionService;

    public DonacionesController(IDonacionService donacionService)
    {
        _donacionService = donacionService;
    }

    // GET /api/donaciones
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DonacionDto>>> GetDonaciones()
    {
        return Ok(await _donacionService.GetDonacionesAsync());
    }

    // GET /api/donaciones/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DonacionDto>> Getdonacion(int id)
    {
        var donacion = await _donacionService.GetDonacionAsync(id);
        return donacion is null ? NotFound() : Ok(donacion);
    }

    // POST /api/donaciones
    [HttpPost]
    public async Task<ActionResult<DonacionDto>> CrearDonacion(CrearDonacionDTO dto)
    {
        var donacion = await _donacionService.CrearDonacionAsync(dto);

        return CreatedAtAction(
            nameof(Getdonacion),
            new { id = donacion.Id },
            donacion
        );
    }

    // PUT /api/donaciones/{id}/reservar
    [HttpPut("{id:int}/reservar")]
    public async Task<IActionResult> ReservarDonacion(int id)
    {
        var (donacion, error) = await _donacionService.ReservarDonacionAsync(id);

        if (donacion is null && error is null)
        {
            return NotFound();
        }

        if (error is not null)
        {
            return BadRequest(error);
        }

        return Ok(donacion);
    }
}
