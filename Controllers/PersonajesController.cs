
using CatalogoGalacticoApi.Dtos;
using CatalogoGalacticoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoGalacticoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonajesController : ControllerBase
{
    private readonly IPersonajeService _personajeService;

    public PersonajesController(IPersonajeService personajeService)
    {
        _personajeService = personajeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PersonajeDto>>> Listar()
    {
        var personajes = await _personajeService.ListarAsync();

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

    [HttpPost]
    public async Task<ActionResult<PersonajeDto>> Crear(
        PersonajeCrearDto dto)
    {
        var personaje = await _personajeService.CrearAsync(dto);

        return CreatedAtAction(
            nameof(ObtenerPorId),
            new { id = personaje.Id },
            personaje);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(
        int id,
        PersonajeCrearDto dto)
    {
        var actualizado = await _personajeService.ActualizarAsync(id, dto);

        if (!actualizado)
        {
            return NotFound();
        }

        return NoContent();
    }
}
