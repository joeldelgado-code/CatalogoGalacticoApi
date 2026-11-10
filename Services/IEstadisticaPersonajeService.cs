
using CatalogoGalacticoApi.Dtos;

namespace CatalogoGalacticoApi.Services;

public interface IEstadisticaPersonajeService
{
    Task<IEnumerable<EstadisticaPersonajeDto>> ListarAsync();

    Task<EstadisticaPersonajeDto?> ObtenerPorPersonajeAsync(int personajeId);

    Task<IEnumerable<EstadisticaPersonajeDto>> RankingPorEventosAsync();
}
