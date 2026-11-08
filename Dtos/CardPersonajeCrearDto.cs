
using System.ComponentModel.DataAnnotations;

namespace CatalogoGalacticoApi.Dtos;

public class CardPersonajeCrearDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Indique un personaje válido")]
    public int PersonajeId { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El poder no puede ser negativo")]
    public int Poder { get; set; }

    [Required(ErrorMessage = "La habilidad especial es obligatoria")]
    [StringLength(150)]
    public string HabilidadEspecial { get; set; } = string.Empty;

    [Required(ErrorMessage = "El arma es obligatoria")]
    [StringLength(100)]
    public string Arma { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "El nivel de peligrosidad no puede ser negativo")]
    public int NivelPeligrosidad { get; set; }

    [Required(ErrorMessage = "La URL de imagen es obligatoria")]
    [Url(ErrorMessage = "La URL de imagen no es válida")]
    public string ImagenUrl { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "La debilidad secreta es obligatoria")]
    [StringLength(500)]
    public string DebilidadSecreta { get; set; } = string.Empty;
}
