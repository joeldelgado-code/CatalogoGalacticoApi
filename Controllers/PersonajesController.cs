
using CatalogoGalacticoApi.Dtos;
using CatalogoGalacticoApi.Models;
using CatalogoGalacticoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoGalacticoApi.Controllers;

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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PersonajeDto>>> Listar(
        [FromQuery] Faccion? faccion,
        [FromQuery] bool? fuerzaSensitivo)
    {
        var personajes = await _personajeService.ListarAsync(
            faccion, fuerzaSensitivo);

        return Ok(personajes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PersonajeDto>> ObtenerPorId(int id)
    {
        var personaje = await _personajeService.ObtenerPorIdAsync(id);

        if (personaje is null)
        {
            return NotFound();
        }

        return Ok(personaje);
    }

    [HttpGet("{id:int}/eventos")]
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

    [HttpPost]
    public async Task<ActionResult<PersonajeDto>> Crear(PersonajeCrearDto dto)
    {
        var personaje = await _personajeService.CrearAsync(dto);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = personaje.Id },
            personaje);
    }

    [HttpPut("{id:int}")]
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
    [HttpGet("ranking")]
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
