namespace CatalogoGalacticoApi.Dtos;

public class SimulacionEventoDto
{
    public string Evento { get; set; } = string.Empty;

    public Dictionary<string, double> PoderPorFaccion { get; set; } = new();

    public string Ganador { get; set; } = string.Empty;

    public double PoderGanador { get; set; }

    public List<PersonajeResumenDto> Fallecidos { get; set; } = new();
}