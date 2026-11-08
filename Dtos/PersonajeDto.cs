
using CatalogoGalacticoApi.Models;

namespace CatalogoGalacticoApi.Dtos;

public class PersonajeDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public Faccion Faccion { get; set; }
    public string Afiliacion { get; set; } = string.Empty;
    public Estado Estado { get; set; }
    public bool FuerzaSensitivo { get; set; }
}
