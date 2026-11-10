
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
    public async Task<IEnumerable<EventoDto>> ListarPorPersonajeAsync(int personajeId)
{
    // Comprobar que el personaje exista
    var existePersonaje = await _db.Personajes
        .AnyAsync(p => p.Id == personajeId);

    if (!existePersonaje)
    {
        throw new KeyNotFoundException(
            "El personaje no existe.");
    }

    // Buscar los eventos en los que participó
    var eventos = await _db.Eventos
        .AsNoTracking()
        .Include(e => e.Participantes)
        .Include(e => e.Fallecidos)
        .Where(e => e.Participantes.Any(p => p.Id == personajeId))
        .OrderBy(e => e.Fecha)
        .ToListAsync();

    return eventos.Select(MapearEvento).ToList();
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

        // Obtener participantes y sus estadísticas
        var participantes = await _db.Personajes
            .Include(p => p.Estadistica)
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
        // en un evento anterior o en otro evento de la misma fecha
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
        // una participación en un evento posterior
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
        // haya fallecido anteriormente en otro evento
        var yaFallecio = await _db.Eventos
            .AnyAsync(e =>
                e.Fallecidos.Any(
                    p => fallecidosIds.Contains(p.Id)));

        if (yaFallecio)
        {
            throw new InvalidOperationException(
                "Un personaje no puede fallecer más de una vez.");
        }

        // Verificar que todos tengan estadísticas
        if (participantes.Any(p => p.Estadistica is null))
        {
            throw new InvalidOperationException(
                "Uno o más personajes no tienen estadísticas registradas.");
        }

        // Obtener los personajes fallecidos
        var fallecidos = participantes
            .Where(p => fallecidosIds.Contains(p.Id))
            .ToList();

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

        // Actualizar estadísticas de los participantes
        foreach (var personaje in participantes)
        {
            var estadisticas = personaje.Estadistica!;

            estadisticas.EventosParticipados++;

            if (fallecidosIds.Contains(personaje.Id))
            {
                personaje.Estado = Estado.Muerto;
                estadisticas.EventoFallecimiento = evento;
            }
            else
            {
                estadisticas.EventosSobrevividos++;
            }

            estadisticas.UltimaActualizacion = DateTime.UtcNow;
        }

        _db.Eventos.Add(evento);

        await _db.SaveChangesAsync();

        return MapearEvento(evento);
    }

    
public async Task<SimulacionEventoDto> SimularAsync(int id)
{
    // 1. Buscar el evento y cargar sus relaciones
    var evento = await _db.Eventos
        .Include(e => e.Participantes)
            .ThenInclude(p => p.Estadistica)
        .Include(e => e.Fallecidos)
        .FirstOrDefaultAsync(e => e.Id == id);

    if (evento is null)
    {
        throw new KeyNotFoundException("El evento no existe.");
    }

    // 2. Evitar simulaciones repetidas
    if (evento.Simulado)
    {
        throw new InvalidOperationException(
            "Este evento ya fue simulado.");
    }

    // 3. Validar cantidad de participantes
    if (evento.Participantes.Count < 2)
    {
        throw new ArgumentException(
            "El evento debe tener al menos 2 participantes.");
    }

    // 4. Comprobar que ningún participante haya fallecido
    if (evento.Participantes.Any(p => p.Estado == Estado.Muerto))
    {
        throw new InvalidOperationException(
            "Un personaje fallecido no puede participar en una simulación.");
    }

    // 5. Verificar que todos tengan estadísticas
    if (evento.Participantes.Any(p => p.Estadistica is null))
    {
        throw new InvalidOperationException(
            "Uno o más personajes no tienen estadísticas registradas.");
    }

    // 6. Obtener las Cards de los participantes
    var participantesIds = evento.Participantes
        .Select(p => p.Id)
        .ToList();

    var cards = await _db.CardsPersonaje
        .Where(c => participantesIds.Contains(c.PersonajeId))
        .ToListAsync();

    if (cards.Count != evento.Participantes.Count)
    {
        throw new ArgumentException(
            "Todos los participantes deben tener una card.");
    }

    // 7. Calcular el poder total por facción
    var poderesPorFaccion = evento.Participantes
        .GroupBy(p => p.Faccion)
        .ToDictionary(
            grupo => grupo.Key,
            grupo => grupo.Sum(p =>
                cards.First(c => c.PersonajeId == p.Id).Poder)
        );

    // 8. Aplicar un factor aleatorio entre 90% y 110%
    var random = Random.Shared;

    var poderesFinales = poderesPorFaccion.ToDictionary(
        x => x.Key,
        x => x.Value * (random.Next(90, 111) / 100.0)
    );

    // 9. Determinar la facción ganadora
    var ganador = poderesFinales
        .OrderByDescending(x => x.Value)
        .ThenBy(x => (int)x.Key)
        .First();

    // 10. Los participantes de las facciones perdedoras fallecen
    var fallecidos = evento.Participantes
        .Where(p => p.Faccion != ganador.Key)
        .ToList();

    foreach (var personaje in fallecidos)
    {
        personaje.Estado = Estado.Muerto;

        evento.Fallecidos.Add(personaje);

        var estadisticas = personaje.Estadistica!;

        // Ya no sobrevivió a este evento
        estadisticas.EventosSobrevividos--;

        // Registrar el evento de fallecimiento
        estadisticas.EventoFallecimiento = evento;

        estadisticas.UltimaActualizacion = DateTime.UtcNow;
    }

    // 11. Guardar el resultado y marcar el evento como simulado
    evento.Resultado =
        $"Ganó la facción {ganador.Key} con un poder de {ganador.Value:F2}. " +
        $"Fallecieron {fallecidos.Count} personajes.";

    evento.Simulado = true;

    await _db.SaveChangesAsync();

    // 12. Construir la respuesta
    return new SimulacionEventoDto
    {
        Evento = evento.Nombre,
        PoderPorFaccion = poderesFinales.ToDictionary(
            x => x.Key.ToString(),
            x => x.Value
        ),
        Ganador = ganador.Key.ToString(),
        PoderGanador = ganador.Value,
        Fallecidos = fallecidos.Select(p => new PersonajeResumenDto
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Estado = p.Estado
        }).ToList()
    };
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
