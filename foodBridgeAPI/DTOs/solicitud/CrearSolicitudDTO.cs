namespace foodBridgeAPI.DTOs.solicitud;
using System.ComponentModel.DataAnnotations;
public class CrearSolicitudDTO
{
    [Required(ErrorMessage = "La donación es obligatoria.")]
    public int DonacionId { get; set; }

    [Required(ErrorMessage = "La fundación es obligatoria.")]
    public int FundacionId { get; set; }
}