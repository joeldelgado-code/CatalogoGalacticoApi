
using CatalogoGalacticoApi.Dtos;
using CatalogoGalacticoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoGalacticoApi.Controllers;

/// <summary>
/// Gestiona la consulta, creación y simulación de eventos del catálogo galáctico.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EventosController : ControllerBase
{
    private readonly IEventoService _eventoService;

    public EventosController(IEventoService eventoService)
    {
        _eventoService = eventoService;
    }

    /// <summary>
    /// Obtiene todos los eventos registrados en el catálogo.
    /// </summary>
    /// <returns>Lista de eventos con sus participantes y resultados.</returns>
    /// <response code="200">Los eventos se obtuvieron correctamente.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EventoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EventoDto>>> Listar()
    {
        var eventos = await _eventoService.ListarAsync();

        return Ok(eventos);
    }

    /// <summary>
    /// Obtiene los detalles de un evento mediante su identificador.
    /// </summary>
    /// <param name="id">Identificador numérico del evento.</param>
    /// <returns>Los datos del evento solicitado.</returns>
    /// <response code="200">El evento fue encontrado.</response>
    /// <response code="404">No existe un evento con ese identificador.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EventoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventoDto>> ObtenerPorId(int id)
    {
        var evento = await _eventoService.ObtenerPorIdAsync(id);

        if (evento is null)
        {
            return NotFound();
        }

        return Ok(evento);
    }

    /// <summary>
    /// Registra un nuevo evento con sus participantes y resultados.
    /// </summary>
    /// <param name="dto">Datos necesarios para registrar el evento.</param>
    /// <returns>El evento creado y la ubicación de su recurso.</returns>
    /// <response code="201">El evento fue creado correctamente.</response>
    /// <response code="400">Los datos enviados contienen valores no válidos.</response>
    /// <response code="404">Uno o más personajes referenciados no existen.</response>
    /// <response code="409">Existe un conflicto con el estado o las relaciones de los personajes.</response>
    [HttpPost]
    [ProducesResponseType(typeof(EventoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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

    /// <summary>
    /// Ejecuta la simulación de un evento existente y determina su resultado.
    /// </summary>
    /// <param name="id">Identificador numérico del evento que se simulará.</param>
    /// <returns>Resultado de la simulación, incluyendo la facción ganadora y los resultados de los participantes.</returns>
    /// <response code="200">La simulación se ejecutó correctamente.</response>
    /// <response code="400">El evento no cumple las condiciones necesarias para simularse.</response>
    /// <response code="404">No existe el evento o alguno de los recursos referenciados.</response>
    /// <response code="409">El estado actual del evento o de sus personajes impide la simulación.</response>
    [HttpPost("{id:int}/simular")]
    [ProducesResponseType(typeof(SimulacionEventoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SimulacionEventoDto>> Simular(int id)
    {
        try
        {
            var resultado = await _eventoService.SimularAsync(id);
            return Ok(resultado);
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
