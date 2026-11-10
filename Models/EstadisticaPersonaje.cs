namespace CatalogoGalacticoApi.Models;

public class EstadisticaPersonaje
{
    public int Id { get; set; }

    public int PersonajeId { get; set; }

    public int EventosParticipados { get; set; }

    public int EventosSobrevividos { get; set; }

    public int? EventoFallecimientoId { get; set; }

    public DateTime UltimaActualizacion { get; set; } = DateTime.UtcNow;

    // Relación con Personaje
    public Personaje Personaje { get; set; } = null!;

    // Evento en el que falleció, si corresponde
    public Evento? EventoFallecimiento { get; set; }
}