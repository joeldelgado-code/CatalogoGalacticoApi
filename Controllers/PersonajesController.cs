
using CatalogoGalacticoApi.Dtos;
using CatalogoGalacticoApi.Models;
using CatalogoGalacticoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoGalacticoApi.Controllers;

/// <summary>
/// Gestiona las operaciones de consulta, creación y actualización de personajes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PersonajesController : ControllerBase
{
    private readonly IPersonajeService _personajeService;
    private readonly IEventoService _eventoService;

    public PersonajesController(
        IPersonajeService personajeService,
        IEventoService eventoService)
    {
        _personajeService = personajeService;
        _eventoService = eventoService;
    }

    /// <summary>
    /// Obtiene la lista de personajes, permitiendo aplicar filtros.
    /// </summary>
    /// <param name="faccion">
    /// Filtro opcional por facción. Debe ser un valor definido en el enum Faccion.
    /// </param>
    /// <param name="fuerzaSensitivo">
    /// Filtro opcional para indicar si el personaje es sensible a la Fuerza.
    /// Acepta true o false.
    /// </param>
    /// <returns>Lista de personajes que cumplen los filtros.</returns>
    /// <response code="200">La lista de personajes se obtuvo correctamente.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PersonajeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PersonajeDto>>> Listar(
        [FromQuery] Faccion? faccion,
        [FromQuery] bool? fuerzaSensitivo)
    {
        var personajes = await _personajeService.ListarAsync(
            faccion, fuerzaSensitivo);

        return Ok(personajes);
    }

    /// <summary>
    /// Obtiene los datos de un personaje mediante su identificador.
    /// </summary>
    /// <param name="id">Identificador numérico del personaje.</param>
    /// <returns>Los datos del personaje solicitado.</returns>
    /// <response code="200">El personaje fue encontrado.</response>
    /// <response code="404">No existe un personaje con ese identificador.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PersonajeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonajeDto>> ObtenerPorId(int id)
    {
        var personaje = await _personajeService.ObtenerPorIdAsync(id);

        if (personaje is null)
        {
            return NotFound();
        }

        return Ok(personaje);
    }

    /// <summary>
    /// Obtiene los eventos en los que ha participado un personaje.
    /// </summary>
    /// <param name="id">Identificador numérico del personaje.</param>
    /// <returns>Lista de eventos relacionados con el personaje.</returns>
    /// <response code="200">Los eventos se obtuvieron correctamente.</response>
    /// <response code="404">El personaje no existe.</response>
    [HttpGet("{id:int}/eventos")]
    [ProducesResponseType(typeof(IEnumerable<EventoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<EventoDto>>> ListarEventos(int id)
    {
        try
        {
            var eventos = await _eventoService.ListarPorPersonajeAsync(id);
            return Ok(eventos);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { mensaje = "El personaje no existe." });
        }
    }

    /// <summary>
    /// Registra un nuevo personaje en el catálogo.
    /// </summary>
    /// <param name="dto">Datos necesarios para crear el personaje.</param>
    /// <returns>El personaje creado y la ubicación de su recurso.</returns>
    /// <response code="201">El personaje fue creado correctamente.</response>
    /// <response code="400">Los datos enviados no superan la validación.</response>
    [HttpPost]
    [ProducesResponseType(typeof(PersonajeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PersonajeDto>> Crear(PersonajeCrearDto dto)
    {
        var personaje = await _personajeService.CrearAsync(dto);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = personaje.Id },
            personaje);
    }

    /// <summary>
    /// Actualiza los datos de un personaje existente.
    /// </summary>
    /// <param name="id">Identificador numérico del personaje que se actualizará.</param>
    /// <param name="dto">Nuevos datos del personaje.</param>
    /// <returns>Respuesta sin contenido si la actualización fue exitosa.</returns>
    /// <response code="204">El personaje fue actualizado correctamente.</response>
    /// <response code="400">Los datos enviados no superan la validación.</response>
    /// <response code="404">No existe un personaje con ese identificador.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(
        int id, PersonajeCrearDto dto)
    {
        var actualizado = await _personajeService.ActualizarAsync(id, dto);

        if (!actualizado)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Obtiene el ranking de personajes ordenado por poder.
    /// </summary>
    /// <param name="por">
    /// Criterio de ordenamiento. Actualmente, el único valor permitido es poder.
    /// </param>
    /// <returns>Lista de personajes ordenada por poder.</returns>
    /// <response code="200">El ranking se obtuvo correctamente.</response>
    /// <response code="400">El criterio de ranking no es válido.</response>
    [HttpGet("ranking")]
    [ProducesResponseType(typeof(IEnumerable<PersonajeRankingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<PersonajeRankingDto>>> ObtenerRanking(
        [FromQuery] string por = "poder")
    {
        if (por != "poder")
        {
            return BadRequest(new
            {
                mensaje = "El criterio de ranking no es válido."
            });
        }

        var ranking = await _personajeService.ObtenerRankingPorPoderAsync();

        return Ok(ranking);
    }
}
