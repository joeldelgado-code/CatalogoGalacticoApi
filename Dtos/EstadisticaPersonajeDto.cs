
namespace CatalogoGalacticoApi.Dtos;

public class EstadisticaPersonajeDto
{
    public int PersonajeId { get; set; }
    public string Personaje { get; set; } = string.Empty;
    public int EventosParticipados { get; set; }
    public int EventosSobrevividos { get; set; }
    public int? EventoFallecimientoId { get; set; }
    public string? EventoFallecimiento { get; set; }
    public DateTime UltimaActualizacion { get; set; }
}
