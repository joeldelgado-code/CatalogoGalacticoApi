namespace CatalogoGalacticoApi.Models;

public class Personaje
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Especie { get; set; } = string.Empty;

    public Faccion Faccion { get; set; }

    public string Afiliacion { get; set; } = string.Empty;

    public Estado Estado { get; set; }= Estado.Vivo;

    public bool FuerzaSensitivo { get; set; }
}
public enum Faccion
{
    Rebelde,
    Imperio,
    Neutral
}

public enum Estado
{
    Vivo,
    Muerto,
    Desconocido
}