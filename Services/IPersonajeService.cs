using CatalogoGalacticoApi.Dtos;

namespace CatalogoGalacticoApi.Services;

public interface IPersonajeService
{
    Task<IEnumerable<PersonajeDto>> ListarAsync();

    Task<PersonajeDto?> ObtenerPorIdAsync(int id);

    Task<PersonajeDto> CrearAsync(PersonajeCrearDto dto);

    Task<bool> ActualizarAsync(int id, PersonajeCrearDto dto);

    
}