
using CatalogoGalacticoApi.Data;
using CatalogoGalacticoApi.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CatalogoGalacticoApi.Services;

public class EstadisticaPersonajeService : IEstadisticaPersonajeService
{
    private readonly AppDbContext _db;

    public EstadisticaPersonajeService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<EstadisticaPersonajeDto>> ListarAsync()
    {
        return await _db.EstadisticasPersonajes
            .AsNoTracking()
            .Include(e => e.Personaje)
            .Include(e => e.EventoFallecimiento)
            .OrderBy(e => e.Personaje.Nombre)
            .Select(e => new EstadisticaPersonajeDto
            {
                PersonajeId = e.PersonajeId,
                Personaje = e.Personaje.Nombre,
                EventosParticipados = e.EventosParticipados,
                EventosSobrevividos = e.EventosSobrevividos,
                EventoFallecimientoId = e.EventoFallecimientoId,
                EventoFallecimiento = e.EventoFallecimiento != null
                    ? e.EventoFallecimiento.Nombre
                    : null,
                UltimaActualizacion = e.UltimaActualizacion
            })
            .ToListAsync();
    }

    public async Task<EstadisticaPersonajeDto?> ObtenerPorPersonajeAsync(int personajeId)
    {
        return await _db.EstadisticasPersonajes
            .AsNoTracking()
            .Where(e => e.PersonajeId == personajeId)
            .Select(e => new EstadisticaPersonajeDto
            {
                PersonajeId = e.PersonajeId,
                Personaje = e.Personaje.Nombre,
                EventosParticipados = e.EventosParticipados,
                EventosSobrevividos = e.EventosSobrevividos,
                EventoFallecimientoId = e.EventoFallecimientoId,
                EventoFallecimiento = e.EventoFallecimiento != null
                    ? e.EventoFallecimiento.Nombre
                    : null,
                UltimaActualizacion = e.UltimaActualizacion
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<EstadisticaPersonajeDto>> RankingPorEventosAsync()
    {
        return await _db.EstadisticasPersonajes
            .AsNoTracking()
            .OrderByDescending(e => e.EventosParticipados)
            .ThenBy(e => e.Personaje.Nombre)
            .Select(e => new EstadisticaPersonajeDto
            {
                PersonajeId = e.PersonajeId,
                Personaje = e.Personaje.Nombre,
                EventosParticipados = e.EventosParticipados,
                EventosSobrevividos = e.EventosSobrevividos,
                EventoFallecimientoId = e.EventoFallecimientoId,
                EventoFallecimiento = e.EventoFallecimiento != null
                    ? e.EventoFallecimiento.Nombre
                    : null,
                UltimaActualizacion = e.UltimaActualizacion
            })
            .ToListAsync();
    }
}
