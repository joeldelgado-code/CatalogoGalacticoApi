
using CatalogoGalacticoApi.Dtos;
using CatalogoGalacticoApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoGalacticoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardsPersonajeController : ControllerBase
{
    private readonly ICardPersonajeService _cardService;

    public CardsPersonajeController(ICardPersonajeService cardService)
    {
        _cardService = cardService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CardPersonajeDto>>> Listar()
    {
        var cards = await _cardService.ListarAsync();

        return Ok(cards);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CardPersonajeDto>> ObtenerPorId(int id)
    {
        var card = await _cardService.ObtenerPorIdAsync(id);

        if (card is null)
        {
            return NotFound();
        }

        return Ok(card);
    }

    [HttpPost]
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
