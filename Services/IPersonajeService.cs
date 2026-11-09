using CatalogoGalacticoApi.Dtos;

using CatalogoGalacticoApi.Models;
namespace CatalogoGalacticoApi.Services;

public interface IPersonajeService
{
    Task<IEnumerable<PersonajeDto>> ListarAsync(Faccion? faccion, bool? fuerzaSensitivo);

    Task<PersonajeDto?> ObtenerPorIdAsync(int id);

    Task<PersonajeDto> CrearAsync(PersonajeCrearDto dto);

    Task<bool> ActualizarAsync(int id, PersonajeCrearDto dto);

    Task<IEnumerable<PersonajeRankingDto>> ObtenerRankingPorPoderAsync();

    
}