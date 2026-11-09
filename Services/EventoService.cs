
using CatalogoGalacticoApi.Data;
using CatalogoGalacticoApi.Dtos;
using CatalogoGalacticoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoGalacticoApi.Services;

public class EventoService : IEventoService
{
    private readonly AppDbContext _db;

    public EventoService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<EventoDto>> ListarAsync()
    {
        var eventos = await _db.Eventos
            .AsNoTracking()
            .Include(e => e.Participantes)
            .Include(e => e.Fallecidos)
            .OrderBy(e => e.Fecha)
            .ToListAsync();

        return eventos.Select(MapearEvento).ToList();
    }

    public async Task<EventoDto?> ObtenerPorIdAsync(int id)
    {
        var evento = await _db.Eventos
            .AsNoTracking()
            .Include(e => e.Participantes)
            .Include(e => e.Fallecidos)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (evento is null)
        {
            return null;
        }

        return MapearEvento(evento);
    }

    public async Task<EventoDto> CrearAsync(EventoCrearDto dto)
    {
        // Validar que exista al menos un participante
        if (dto.ParticipantesIds.Count == 0)
        {
            throw new ArgumentException(
                "El evento debe tener al menos un participante.");
        }

        // Evitar IDs repetidos
        if (dto.ParticipantesIds.Distinct().Count()
            != dto.ParticipantesIds.Count
            || dto.FallecidosIds.Distinct().Count()
            != dto.FallecidosIds.Count)
        {
            throw new ArgumentException(
                "No se permiten personajes repetidos.");
        }

        // Los fallecidos deben ser participantes
        if (!dto.FallecidosIds.All(
            id => dto.ParticipantesIds.Contains(id)))
        {
            throw new ArgumentException(
                "Todos los fallecidos deben ser participantes del evento.");
        }

        // Obtener los participantes desde la base de datos
        var participantes = await _db.Personajes
            .Where(p => dto.ParticipantesIds.Contains(p.Id))
            .ToListAsync();

        if (participantes.Count != dto.ParticipantesIds.Count)
        {
            throw new KeyNotFoundException(
                "Uno o más participantes no existen.");
        }

        var participantesIds = dto.ParticipantesIds;
        var fallecidosIds = dto.FallecidosIds;

        // Comprobar que ningún participante haya fallecido
        // en un evento anterior o en otro evento de la misma fecha.
        var existeFallecidoAnterior = await _db.Eventos
            .AnyAsync(e =>
                e.Fecha <= dto.Fecha
                && e.Fallecidos.Any(
                    p => participantesIds.Contains(p.Id)));

        if (existeFallecidoAnterior)
        {
            throw new InvalidOperationException(
                "Un personaje ya fallecido no puede participar en este evento.");
        }

        // Comprobar que los fallecidos no tengan
        // una participación en un evento posterior.
        var existeParticipacionPosterior = await _db.Eventos
            .AnyAsync(e =>
                e.Fecha >= dto.Fecha
                && e.Participantes.Any(
                    p => fallecidosIds.Contains(p.Id)));

        if (existeParticipacionPosterior)
        {
            throw new InvalidOperationException(
                "No se puede registrar un fallecimiento porque el personaje tiene participaciones posteriores.");
        }

        // Comprobar que ninguno de los fallecidos
        // haya fallecido anteriormente en otro evento.
        var yaFallecio = await _db.Eventos
            .AnyAsync(e =>
                e.Fallecidos.Any(
                    p => fallecidosIds.Contains(p.Id)));

        if (yaFallecio)
        {
            throw new InvalidOperationException(
                "Un personaje no puede fallecer más de una vez.");
        }

        // Obtener los personajes fallecidos
        var fallecidos = participantes
            .Where(p => fallecidosIds.Contains(p.Id))
            .ToList();

        // Actualizar el estado de los fallecidos
        foreach (var personaje in fallecidos)
        {
            personaje.Estado = Estado.Muerto;
        }

        // Generar el resultado del evento
        string resultado = fallecidos.Count > 0
            ? $"Se registraron {fallecidos.Count} fallecido(s)."
            : "No se registraron fallecidos.";

        // Crear el evento
        var evento = new Evento
        {
            Nombre = dto.Nombre,
            Fecha = dto.Fecha,
            Ubicacion = dto.Ubicacion,
            Descripcion = dto.Descripcion,
            Participantes = participantes,
            Fallecidos = fallecidos,
            Resultado = resultado
        };

        _db.Eventos.Add(evento);

        await _db.SaveChangesAsync();

        return MapearEvento(evento);
    }

    private static EventoDto MapearEvento(Evento evento)
    {
        return new EventoDto
        {
            Id = evento.Id,
            Nombre = evento.Nombre,
            Fecha = evento.Fecha,
            Ubicacion = evento.Ubicacion,
            Descripcion = evento.Descripcion,
            Participantes = evento.Participantes
                .Select(p => new PersonajeResumenDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Estado = p.Estado
                })
                .ToList(),
            Fallecidos = evento.Fallecidos
                .Select(p => new PersonajeResumenDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Estado = p.Estado
                })
                .ToList(),
            Resultado = evento.Resultado
        };
    }
}
