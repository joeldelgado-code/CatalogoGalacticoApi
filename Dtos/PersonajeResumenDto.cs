
using CatalogoGalacticoApi.Models;

namespace CatalogoGalacticoApi.Dtos;

public class PersonajeResumenDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public Estado Estado { get; set; }
}
