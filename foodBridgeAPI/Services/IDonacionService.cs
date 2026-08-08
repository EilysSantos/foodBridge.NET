using foodBridgeAPI.DTOs.donacion;

namespace foodBridgeAPI.Services;

public interface IDonacionService
{
    Task<IEnumerable<DonacionDto>> GetDonacionesAsync();
    Task<DonacionDto?> GetDonacionAsync(int id);
    Task<DonacionDto> CrearDonacionAsync(CrearDonacionDTO dto);
    Task<(DonacionDto? Donacion, string? Error)> ReservarDonacionAsync(int id);
}
