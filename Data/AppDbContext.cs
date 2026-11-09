
using CatalogoGalacticoApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CatalogoGalacticoApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Personaje> Personajes => Set<Personaje>();
    public DbSet<CardPersonaje> CardsPersonaje => Set<CardPersonaje>();
    public DbSet<Evento> Eventos => Set<Evento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // PERSONAJE
        modelBuilder.Entity<Personaje>(e =>
        {
            e.Property(p => p.Nombre).IsRequired().HasMaxLength(50);

            e.Property(p => p.Especie).IsRequired().HasMaxLength(80);

            e.Property(p => p.Afiliacion).HasMaxLength(100);

            e.Property(p => p.Faccion).IsRequired();
            e.Property(p => p.Estado).IsRequired();
        });

         // CARD PERSONAJE
        modelBuilder.Entity<CardPersonaje>(e =>
        {
            e.Property(c => c.HabilidadEspecial)
                .IsRequired()
                .HasMaxLength(200);

            e.Property(c => c.Arma)
                .IsRequired()
                .HasMaxLength(100);

            e.Property(c => c.ImagenUrl)
                .HasMaxLength(500);

            e.Property(c => c.DebilidadSecretaCifrada)
                .IsRequired()
                .HasMaxLength(1000);

            // Relación 1:1 con Personaje
            e.HasOne(c => c.Personaje)
                .WithOne()
                .HasForeignKey<CardPersonaje>(c => c.PersonajeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // EVENTO
        modelBuilder.Entity<Evento>(e =>
        {
            e.Property(ev => ev.Nombre) .IsRequired().HasMaxLength(120);

            e.Property(ev => ev.Ubicacion).IsRequired().HasMaxLength(150);

            e.Property(ev => ev.Descripcion).HasMaxLength(500);

            e.Property(ev => ev.Resultado).HasMaxLength(500);

            // Participantes: muchos a muchos
            e.HasMany(ev => ev.Participantes)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "EventoParticipante",
                    j => j.HasOne<Personaje>()
                        .WithMany()
                        .HasForeignKey("PersonajeId")
                        .OnDelete(DeleteBehavior.Restrict),
                    j => j.HasOne<Evento>()
                        .WithMany()
                        .HasForeignKey("EventoId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("EventoId", "PersonajeId");
                        j.ToTable("EventoParticipantes");
                    });

            // Fallecidos: muchos a muchos con Personaje único
            e.HasMany(ev => ev.Fallecidos)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "EventoFallecido",
                    j => j.HasOne<Personaje>()
                        .WithMany()
                        .HasForeignKey("PersonajeId")
                        .OnDelete(DeleteBehavior.Restrict),
                    j => j.HasOne<Evento>()
                        .WithMany()
                        .HasForeignKey("EventoId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("EventoId", "PersonajeId");

                        // Un personaje solo puede fallecer una vez
                        j.HasIndex("PersonajeId").IsUnique();

                        j.ToTable("EventoFallecidos");
                    });
        });
    }
}
