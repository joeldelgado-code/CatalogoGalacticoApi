
using CatalogoGalacticoApi.Data;
using CatalogoGalacticoApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// CONEXIÓN A MYSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Falta la cadena de conexión DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 46)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
    ));

// CONTROLLERS Y OPENAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// INYECCIÓN DE DEPENDENCIAS
builder.Services.AddScoped<IPersonajeService, PersonajeService>();
builder.Services.AddScoped<ICardPersonajeService, CardPersonajeService>();
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddSingleton<ICifradoService, CifradoService>();

var app = builder.Build();

// CONFIGURACIÓN DE SWAGGER
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Catálogo Galáctico API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
