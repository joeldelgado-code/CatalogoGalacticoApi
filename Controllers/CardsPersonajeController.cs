
using CatalogoGalacticoApi.Dtos;
using CatalogoGalacticoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoGalacticoApi.Controllers;

/// <summary>
/// Gestiona las operaciones de consulta y creación de las tarjetas de personajes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CardsPersonajeController : ControllerBase
{
    private readonly ICardPersonajeService _cardService;

    public CardsPersonajeController(ICardPersonajeService cardService)
    {
        _cardService = cardService;
    }

    /// <summary>
    /// Obtiene todas las tarjetas de personajes registradas.
    /// </summary>
    /// <returns>Lista de tarjetas con sus datos.</returns>
    /// <response code="200">La lista de tarjetas se obtuvo correctamente.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CardPersonajeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CardPersonajeDto>>> Listar()
    {
        var cards = await _cardService.ListarAsync();
        return Ok(cards);
    }

    /// <summary>
    /// Obtiene una tarjeta mediante su identificador.
    /// </summary>
    /// <param name="id">Identificador numérico de la tarjeta.</param>
    /// <returns>Los datos de la tarjeta solicitada.</returns>
    /// <response code="200">La tarjeta fue encontrada.</response>
    /// <response code="404">No existe una tarjeta con ese identificador.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CardPersonajeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CardPersonajeDto>> ObtenerPorId(int id)
    {
        var card = await _cardService.ObtenerPorIdAsync(id);

        if (card is null)
        {
            return NotFound();
        }

        return Ok(card);
    }

    /// <summary>
    /// Crea una tarjeta y la asocia a un personaje existente.
    /// </summary>
    /// <param name="dto">Datos necesarios para crear la tarjeta.</param>
    /// <returns>La tarjeta creada y la ubicación de su recurso.</returns>
    /// <response code="201">La tarjeta fue creada correctamente.</response>
    /// <response code="400">Los datos enviados no superan la validación.</response>
    /// <response code="404">El personaje asociado no existe.</response>
    /// <response code="409">Existe un conflicto que impide crear la tarjeta.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CardPersonajeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CardPersonajeDto>> Crear(
        CardPersonajeCrearDto dto)
    {
        try
        {
            var card = await _cardService.CrearAsync(dto);

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = card.Id },
                card);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }
}
