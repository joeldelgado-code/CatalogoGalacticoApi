
using CatalogoGalacticoApi.Dtos;

namespace CatalogoGalacticoApi.Services;

public interface IEventoService
{
    Task<IEnumerable<EventoDto>> ListarAsync();

    Task<EventoDto?> ObtenerPorIdAsync(int id);
    Task<IEnumerable<EventoDto>> ListarPorPersonajeAsync(int personajeId);

    Task<EventoDto> CrearAsync(EventoCrearDto dto);

    Task<SimulacionEventoDto> SimularAsync(int id);
}
