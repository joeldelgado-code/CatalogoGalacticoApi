
using CatalogoGalacticoApi.Dtos;

namespace CatalogoGalacticoApi.Services;

public interface IEventoService
{
    Task<IEnumerable<EventoDto>> ListarAsync();

    Task<EventoDto?> ObtenerPorIdAsync(int id);

    Task<EventoDto> CrearAsync(EventoCrearDto dto);
}
