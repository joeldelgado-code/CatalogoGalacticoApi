
namespace CatalogoGalacticoApi.Dtos;

public class EventoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Ubicacion { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public List<PersonajeResumenDto> Participantes { get; set; } = new();
    public List<PersonajeResumenDto> Fallecidos { get; set; } = new();
    public string Resultado { get; set; } = string.Empty;
}
