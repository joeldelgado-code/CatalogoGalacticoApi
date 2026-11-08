
using System.ComponentModel.DataAnnotations;

namespace CatalogoGalacticoApi.Dtos;

public class EventoCrearDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "La ubicación es obligatoria")]
    [StringLength(150)]
    public string Ubicacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    public List<int> ParticipantesIds { get; set; } = new();
    
    public List<int> FallecidosIds { get; set; } = new();
}
