# Catálogo Galáctico API

API REST desarrollada con ASP.NET Core Web API para gestionar un catálogo galáctico de personajes, sus tarjetas de información y los eventos en los que participan.

El proyecto utiliza una arquitectura basada en controladores, servicios, DTOs y Entity Framework Core para separar las responsabilidades y facilitar el mantenimiento de la aplicación.

## Tecnologías utilizadas

- **C# y .NET 10:** desarrollo de la API.
- **ASP.NET Core Web API:** creación de endpoints REST mediante controladores.
- **Entity Framework Core:** acceso y persistencia de datos.
- **MySQL 8:** sistema gestor de base de datos.
- **Pomelo.EntityFrameworkCore.MySql:** proveedor de Entity Framework Core para MySQL.
- **Swagger / OpenAPI:** documentación y pruebas de los endpoints.
- **AES-GCM:** cifrado de información sensible.
- **Git y GitHub:** control de versiones.

## Funcionalidades

### Gestión de personajes

- Registrar personajes.
- Consultar todos los personajes.
- Filtrar personajes por facción y sensibilidad a la Fuerza.
- Consultar un personaje por su identificador.
- Actualizar los datos de un personaje.
- Consultar los eventos en los que participó.
- Obtener el ranking de personajes por poder.

### Gestión de tarjetas de personajes

- Registrar tarjetas asociadas a personajes existentes.
- Consultar todas las tarjetas.
- Consultar una tarjeta por su identificador.
- Proteger la debilidad secreta mediante cifrado AES-GCM.

### Gestión de eventos

- Registrar eventos con sus participantes y fallecidos.
- Consultar todos los eventos.
- Consultar un evento por su identificador.
- Simular eventos y determinar la facción ganadora.
- Actualizar el estado de los personajes y sus estadísticas durante los eventos.

### Sistema de estadísticas

- Consultar las estadísticas de todos los personajes.
- Consultar las estadísticas de un personaje específico.
- Obtener un ranking según la cantidad de eventos en los que participó cada personaje.
- Registrar la cantidad de eventos participados y sobrevividos.
- Identificar el evento en el que falleció un personaje.

## Arquitectura del proyecto

El proyecto está organizado en las siguientes carpetas:

| Carpeta | Responsabilidad |
|---|---|
| `Controllers` | Recibe las solicitudes HTTP y devuelve las respuestas. |
| `Services` | Contiene la lógica de negocio de la aplicación. |
| `Models` | Define las entidades del dominio. |
| `Dtos` | Define los objetos utilizados para recibir y devolver información. |
| `Data` | Contiene el contexto de Entity Framework Core. |
| `Migrations` | Contiene las migraciones de la base de datos. |

La inyección de dependencias permite que los controladores utilicen las interfaces de los servicios sin depender directamente de sus implementaciones.

## Requisitos previos

Para ejecutar el proyecto se necesita:

- .NET 10 SDK.
- MySQL Server 8.
- Git.
- Herramienta de línea de comandos de Entity Framework Core (`dotnet-ef`).

## Instalación y configuración

### 1. Clonar el repositorio

```bash
git clone https://github.com/joeldelgado-code/CatalogoGalacticoApi.git
cd CatalogoGalacticoApi
```

### 2. Configurar la base de datos

Crear la base de datos en MySQL:

```sql
CREATE DATABASE catalogo_galactico_db;
```

La aplicación utiliza una cadena de conexión almacenada mediante **User Secrets**, evitando guardar credenciales en el repositorio.

Desde la carpeta del proyecto, configurar la conexión:

```powershell
dotnet user-secrets init

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=catalogo_galactico_db;User=catalogo_user;Password=TU_CONTRASEÑA"
```

Reemplazar `TU_CONTRASEÑA` y, si corresponde, el usuario por las credenciales de MySQL de la instalación local.

### 3. Configurar la clave de cifrado

La aplicación utiliza AES-GCM para proteger el campo `DebilidadSecreta` de las tarjetas de personajes.

Se necesita una clave de 32 bytes codificada en Base64. Puede generarse y almacenarse con PowerShell:

```powershell
$clave = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(32))

dotnet user-secrets set "Cifrado:Clave" $clave
```

La clave debe mantenerse privada y no debe subirse al repositorio.

**Importante:** si se pierde o cambia la clave, no se podrán descifrar los datos que fueron cifrados con la clave anterior.

### 4. Instalar la herramienta de migraciones

Si todavía no está instalada:

```powershell
dotnet tool install --global dotnet-ef
```

### 5. Aplicar las migraciones

```powershell
dotnet ef database update
```

Este comando crea o actualiza las tablas de acuerdo con las migraciones del proyecto.

### 6. Ejecutar la aplicación

```powershell
dotnet restore
dotnet build
dotnet run
```

La aplicación se ejecuta utilizando el perfil configurado en el proyecto. La dirección HTTPS se muestra en la consola al iniciar.

## Documentación de la API

La documentación interactiva está disponible mediante Swagger UI en:

```text
/swagger
```

Desde Swagger se pueden consultar los endpoints, revisar los parámetros, enviar solicitudes y visualizar las respuestas HTTP.

## Endpoints disponibles

Los endpoints están organizados en cuatro controladores.

### Personajes — `/api/Personajes`

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/Personajes` | Lista los personajes y permite aplicar filtros. |
| GET | `/api/Personajes/{id}` | Obtiene un personaje por su identificador. |
| GET | `/api/Personajes/{id}/eventos` | Lista los eventos de un personaje. |
| POST | `/api/Personajes` | Registra un personaje. |
| PUT | `/api/Personajes/{id}` | Actualiza un personaje. |
| GET | `/api/Personajes/ranking?por=poder` | Obtiene el ranking por poder. |

**Filtros disponibles:**

- `faccion`: permite filtrar por `Rebelde`, `Imperio` o `Neutral`.
- `fuerzaSensitivo`: acepta `true` o `false`.

El parámetro `por` del ranking actualmente acepta únicamente `poder`.

**Ejemplo de creación:**

```json
{
  "nombre": "Luke Skywalker",
  "especie": "Humano",
  "faccion": 0,
  "afiliacion": "Alianza Rebelde",
  "fuerzaSensitivo": true
}
```

Valores de `Faccion`:

| Valor | Facción |
|---|---|
| 0 | Rebelde |
| 1 | Imperio |
| 2 | Neutral |

### Tarjetas de personajes — `/api/CardsPersonaje`

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/CardsPersonaje` | Lista todas las tarjetas. |
| GET | `/api/CardsPersonaje/{id}` | Obtiene una tarjeta por su identificador. |
| POST | `/api/CardsPersonaje` | Crea una tarjeta asociada a un personaje. |

**Ejemplo de creación:**

```json
{
  "personajeId": 1,
  "poder": 95,
  "habilidadEspecial": "Dominio de la Fuerza",
  "arma": "Sable de luz",
  "nivelPeligrosidad": 8,
  "imagenUrl": "https://example.com/luke.jpg",
  "debilidadSecreta": "Su apego a sus seres queridos"
}
```

La debilidad secreta se cifra antes de almacenarse y se descifra al construir las respuestas de las tarjetas.

### Eventos — `/api/Eventos`

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/Eventos` | Lista todos los eventos. |
| GET | `/api/Eventos/{id}` | Obtiene un evento por su identificador. |
| POST | `/api/Eventos` | Registra un evento. |
| POST | `/api/Eventos/{id}/simular` | Ejecuta la simulación de un evento existente. |

**Ejemplo de creación:**

```json
{
  "nombre": "Batalla de Endor",
  "fecha": "2026-11-09T15:00:00",
  "ubicacion": "Endor",
  "descripcion": "Enfrentamiento entre las fuerzas rebeldes y el Imperio.",
  "participantesIds": [1, 2, 3],
  "fallecidosIds": []
}
```

La simulación determina la facción ganadora y actualiza los resultados de los personajes participantes.

### Estadísticas — `/api/estadisticas`

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/estadisticas` | Lista las estadísticas de todos los personajes. |
| GET | `/api/estadisticas/{personajeId}` | Obtiene las estadísticas de un personaje. |
| GET | `/api/estadisticas/ranking/eventos` | Obtiene el ranking por cantidad de eventos participados. |

El ranking se ordena de mayor a menor cantidad de participaciones. En caso de empate, se utiliza el nombre del personaje en orden alfabético.

## Códigos de respuesta HTTP

La API utiliza códigos HTTP para indicar el resultado de las operaciones:

| Código | Significado |
|---|---|
| 200 OK | La solicitud se procesó correctamente. |
| 201 Created | El recurso fue creado correctamente. |
| 204 No Content | La actualización se realizó correctamente sin devolver contenido. |
| 400 Bad Request | Los datos enviados no son válidos. |
| 404 Not Found | No se encontró el recurso solicitado. |
| 409 Conflict | Existe un conflicto con el estado o las relaciones de los datos. |

## Seguridad y protección de datos

- Las credenciales de conexión a MySQL se almacenan mediante User Secrets.
- La clave de cifrado se almacena fuera del código fuente.
- El campo `DebilidadSecreta` se protege mediante AES-GCM antes de guardarse en la base de datos.
- Se utilizan DTOs para controlar los datos recibidos y devueltos por la API.
- Se aplican validaciones mediante Data Annotations en los DTOs de entrada.

## Autor

Proyecto académico desarrollado individualmente para la asignatura **Tecnologías Web 1**, de la Universidad Católica Boliviana, sede Cochabamba.

## Repositorio

[Catálogo Galáctico API — GitHub](https://github.com/joeldelgado-code/CatalogoGalacticoApi)
