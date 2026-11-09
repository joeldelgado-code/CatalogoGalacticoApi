
using CatalogoGalacticoApi.Data;
using CatalogoGalacticoApi.Dtos;
using CatalogoGalacticoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoGalacticoApi.Services;

public class CardPersonajeService : ICardPersonajeService
{
    private readonly AppDbContext _db;
    private readonly ICifradoService _cifrado;

    public CardPersonajeService(
        AppDbContext db,
        ICifradoService cifrado)
    {
        _db = db;
        _cifrado = cifrado;
    }

    public async Task<IEnumerable<CardPersonajeDto>> ListarAsync()
    {
        var cards = await _db.CardsPersonaje
            .AsNoTracking()
            .Include(c => c.Personaje)
            .OrderBy(c => c.Id)
            .ToListAsync();

        return cards.Select(c => new CardPersonajeDto
        {
            Id = c.Id,
            PersonajeId = c.PersonajeId,
            NombrePersonaje = c.Personaje.Nombre,
            Poder = c.Poder,
            HabilidadEspecial = c.HabilidadEspecial,
            Arma = c.Arma,
            NivelPeligrosidad = c.NivelPeligrosidad,
            ImagenUrl = c.ImagenUrl,
            DebilidadSecreta = _cifrado.Descifrar(
                c.DebilidadSecretaCifrada)
        });
    }

    public async Task<CardPersonajeDto?> ObtenerPorIdAsync(int id)
    {
        var card = await _db.CardsPersonaje
            .AsNoTracking()
            .Include(c => c.Personaje)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (card is null)
        {
            return null;
        }

        return new CardPersonajeDto
        {
            Id = card.Id,
            PersonajeId = card.PersonajeId,
            NombrePersonaje = card.Personaje.Nombre,
            Poder = card.Poder,
            HabilidadEspecial = card.HabilidadEspecial,
            Arma = card.Arma,
            NivelPeligrosidad = card.NivelPeligrosidad,
            ImagenUrl = card.ImagenUrl,
            DebilidadSecreta = _cifrado.Descifrar(
                card.DebilidadSecretaCifrada)
        };
    }

    public async Task<CardPersonajeDto> CrearAsync(CardPersonajeCrearDto dto)
    {
        var personajeExiste = await _db.Personajes
            .AnyAsync(p => p.Id == dto.PersonajeId);

        if (!personajeExiste)
        {
            throw new KeyNotFoundException(
                "No existe el personaje indicado.");
        }

        var yaTieneCard = await _db.CardsPersonaje
            .AnyAsync(c => c.PersonajeId == dto.PersonajeId);

        if (yaTieneCard)
        {
            throw new InvalidOperationException(
                "El personaje ya tiene una carta.");
        }

        var card = new CardPersonaje
        {
            PersonajeId = dto.PersonajeId,
            Poder = dto.Poder,
            HabilidadEspecial = dto.HabilidadEspecial,
            Arma = dto.Arma,
            NivelPeligrosidad = dto.NivelPeligrosidad,
            ImagenUrl = dto.ImagenUrl,
            DebilidadSecretaCifrada = _cifrado.Cifrar(
                dto.DebilidadSecreta)
        };

        _db.CardsPersonaje.Add(card);
        await _db.SaveChangesAsync();

        return await ObtenerPorIdAsync(card.Id)
            ?? throw new InvalidOperationException(
                "No se pudo recuperar la carta creada.");
    }
}
