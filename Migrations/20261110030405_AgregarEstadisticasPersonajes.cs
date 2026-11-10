
using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogoGalacticoApi.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEstadisticasPersonajes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Agregar la columna que indica si un evento ya fue simulado.
            migrationBuilder.AddColumn<bool>(
                name: "Simulado",
                table: "Eventos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            // Crear la tabla de estadísticas.
            migrationBuilder.CreateTable(
                name: "EstadisticasPersonajes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),

                    PersonajeId = table.Column<int>(type: "int", nullable: false),

                    EventosParticipados = table.Column<int>(type: "int", nullable: false),

                    EventosSobrevividos = table.Column<int>(type: "int", nullable: false),

                    EventoFallecimientoId = table.Column<int>(type: "int", nullable: true),

                    UltimaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadisticasPersonajes", x => x.Id);

                    table.ForeignKey(
                        name: "FK_EstadisticasPersonajes_Eventos_EventoFallecimientoId",
                        column: x => x.EventoFallecimientoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_EstadisticasPersonajes_Personajes_PersonajeId",
                        column: x => x.PersonajeId,
                        principalTable: "Personajes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Crear índices.
            migrationBuilder.CreateIndex(
                name: "IX_EstadisticasPersonajes_EventoFallecimientoId",
                table: "EstadisticasPersonajes",
                column: "EventoFallecimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_EstadisticasPersonajes_PersonajeId",
                table: "EstadisticasPersonajes",
                column: "PersonajeId",
                unique: true);

            // Reconocer los eventos que ya fueron simulados.
            migrationBuilder.Sql("""
                UPDATE Eventos
                SET Simulado = 1
                WHERE Resultado LIKE 'Ganó la facción %';
                """);

            // Inicializar las estadísticas de los personajes existentes.
            migrationBuilder.Sql("""
                INSERT INTO EstadisticasPersonajes
                (
                    PersonajeId,
                    EventosParticipados,
                    EventosSobrevividos,
                    EventoFallecimientoId,
                    UltimaActualizacion
                )
                SELECT
                    p.Id,

                    (
                        SELECT COUNT(*)
                        FROM EventoParticipantes ep
                        WHERE ep.PersonajeId = p.Id
                    ),

                    (
                        SELECT COUNT(*)
                        FROM EventoParticipantes ep
                        WHERE ep.PersonajeId = p.Id
                          AND NOT EXISTS
                          (
                              SELECT 1
                              FROM EventoFallecidos ef
                              WHERE ef.EventoId = ep.EventoId
                                AND ef.PersonajeId = ep.PersonajeId
                          )
                    ),

                    (
                        SELECT ef.EventoId
                        FROM EventoFallecidos ef
                        WHERE ef.PersonajeId = p.Id
                        LIMIT 1
                    ),

                    UTC_TIMESTAMP(6)

                FROM Personajes p;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstadisticasPersonajes");

            migrationBuilder.DropColumn(
                name: "Simulado",
                table: "Eventos");
        }
    }
}
