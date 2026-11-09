
using CatalogoGalacticoApi.Dtos;
using CatalogoGalacticoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoGalacticoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventosController : ControllerBase
{
    private readonly IEventoService _eventoService;

    public EventosController(IEventoService eventoService)
    {
        _eventoService = eventoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventoDto>>> Listar()
    {
        var eventos = await _eventoService.ListarAsync();

        return Ok(eventos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EventoDto>> ObtenerPorId(int id)
    {
        var evento = await _eventoService.ObtenerPorIdAsync(id);

        if (evento is null)
        {
            return NotFound();
        }

        return Ok(evento);
    }

    [HttpPost]
    public async Task<ActionResult<EventoDto>> Crear(EventoCrearDto dto)
    {
        try
        {
            var evento = await _eventoService.CrearAsync(dto);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = evento.Id },
                evento);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }
}
