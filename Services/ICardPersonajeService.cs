using CatalogoGalacticoApi.Dtos;

namespace CatalogoGalacticoApi.Services;

public interface ICardPersonajeService
{
    Task<IEnumerable<CardPersonajeDto>> ListarAsync();

    Task<CardPersonajeDto?> ObtenerPorIdAsync(int id);

    Task<CardPersonajeDto> CrearAsync(CardPersonajeCrearDto dto);
}