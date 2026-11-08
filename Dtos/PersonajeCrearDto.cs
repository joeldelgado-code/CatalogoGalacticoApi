
using System.ComponentModel.DataAnnotations;
using CatalogoGalacticoApi.Models;

namespace CatalogoGalacticoApi.Dtos;

public class PersonajeCrearDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La especie es obligatoria")]
    [StringLength(80)]
    public string Especie { get; set; } = string.Empty;

    public Faccion Faccion { get; set; }

    [Required(ErrorMessage = "La afiliación es obligatoria")]
    [StringLength(100)]
    public string Afiliacion { get; set; } = string.Empty;

    public bool FuerzaSensitivo { get; set; }
}
