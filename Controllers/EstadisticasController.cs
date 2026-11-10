
using CatalogoGalacticoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoGalacticoApi.Controllers;

/// <summary>
/// Permite consultar las estadísticas y el ranking de participación de los personajes.
/// </summary>
[ApiController]
[Route("api/estadisticas")]
public class EstadisticasController : ControllerBase
{
    private readonly IEstadisticaPersonajeService _service;

    public EstadisticasController(IEstadisticaPersonajeService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtiene las estadísticas de todos los personajes registrados.
    /// </summary>
    /// <returns>Lista de estadísticas de los personajes.</returns>
    /// <response code="200">Las estadísticas se obtuvieron correctamente.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
    {
        var estadisticas = await _service.ListarAsync();
        return Ok(estadisticas);
    }

    /// <summary>
    /// Obtiene las estadísticas de un personaje específico.
    /// </summary>
    /// <param name="personajeId">Identificador numérico del personaje.</param>
    /// <returns>Estadísticas de participación, supervivencia y fallecimiento del personaje.</returns>
    /// <response code="200">Las estadísticas fueron encontradas.</response>
    /// <response code="404">No se encontraron estadísticas para ese personaje.</response>
    [HttpGet("{personajeId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorPersonaje(int personajeId)
    {
        var estadistica = await _service.ObtenerPorPersonajeAsync(personajeId);

        if (estadistica is null)
            return NotFound(new { mensaje = "No se encontraron estadísticas para este personaje." });

        return Ok(estadistica);
    }

    /// <summary>
    /// Obtiene el ranking de personajes ordenado por cantidad de eventos en los que participaron.
    /// </summary>
    /// <returns>Lista de personajes ordenada de mayor a menor participación.</returns>
    /// <response code="200">El ranking se obtuvo correctamente.</response>
    [HttpGet("ranking/eventos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RankingPorEventos()
    {
        var ranking = await _service.RankingPorEventosAsync();
        return Ok(ranking);
    }
}
