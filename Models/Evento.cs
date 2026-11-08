namespace CatalogoGalacticoApi.Models;

public class Evento
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public string Ubicacion { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public List<Personaje> Participantes { get; set; } = new();
    public List<Personaje> Fallecidos { get; set; } = new();
    public string Resultado { get; set; } = string.Empty;
}