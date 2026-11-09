using CatalogoGalacticoApi.Data;
using CatalogoGalacticoApi.Dtos;
using CatalogoGalacticoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoGalacticoApi.Services;

public class PersonajeService : IPersonajeService
{
    private readonly AppDbContext _db;

    public PersonajeService(AppDbContext db)
    {
        _db = db;
    }
    public async Task<IEnumerable<PersonajeDto>> ListarAsync()
    {
        return await _db.Personajes
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .Select(p => new PersonajeDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Especie = p.Especie,
                Faccion = p.Faccion,
                Afiliacion = p.Afiliacion,
                Estado = p.Estado,
                FuerzaSensitivo = p.FuerzaSensitivo
            })
            .ToListAsync();
    }
    public async Task<PersonajeDto?> ObtenerPorIdAsync(int id)
    {
        return await _db.Personajes
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PersonajeDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Especie = p.Especie,
                Faccion = p.Faccion,
                Afiliacion = p.Afiliacion,
                Estado = p.Estado,
                FuerzaSensitivo = p.FuerzaSensitivo
            })
            .FirstOrDefaultAsync();
    }
    public async Task<PersonajeDto> CrearAsync(PersonajeCrearDto dto)
    {
        var personaje = new Personaje
        {
            Nombre = dto.Nombre,
            Especie = dto.Especie,
            Faccion = dto.Faccion,
            Afiliacion = dto.Afiliacion,
            Estado = Estado.Vivo,
            FuerzaSensitivo = dto.FuerzaSensitivo
        };

        _db.Personajes.Add(personaje);
        await _db.SaveChangesAsync();

        return new PersonajeDto
        {
            Id = personaje.Id,
            Nombre = personaje.Nombre,
            Especie = personaje.Especie,
            Faccion = personaje.Faccion,
            Afiliacion = personaje.Afiliacion,
            Estado = personaje.Estado,
            FuerzaSensitivo = personaje.FuerzaSensitivo
        };
    }
    public async Task<bool> ActualizarAsync(int id, PersonajeCrearDto dto)
    {
        var personaje = await _db.Personajes.FindAsync(id);

        if (personaje is null)
        {
            return false;
        }

        personaje.Nombre = dto.Nombre;
        personaje.Especie = dto.Especie;
        personaje.Faccion = dto.Faccion;
        personaje.Afiliacion = dto.Afiliacion;
        personaje.FuerzaSensitivo = dto.FuerzaSensitivo;

        await _db.SaveChangesAsync();

        return true;
    }
}