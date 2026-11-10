Declaración de uso de inteligencia artificial
Herramienta utilizada
Durante el desarrollo del proyecto Catálogo Galáctico API se utilizó ChatGPT como herramienta de apoyo para resolver dudas técnicas, analizar alternativas de implementación, revisar código y mejorar determinadas funcionalidades.
Organización y desarrollo del proyecto
El proyecto se organizó mediante una separación de responsabilidades utilizando las siguientes estructuras:
Models: representación de las entidades del sistema.
Dtos: objetos utilizados para transferir información mediante la API.
Controllers: gestión de las solicitudes HTTP.
Services: implementación de la lógica de negocio.
Data: configuración del contexto de base de datos mediante AppDbContext.
La implementación de estas estructuras formó parte del desarrollo del proyecto, mientras que ChatGPT se utilizó como apoyo en consultas y revisiones puntuales.
Áreas en las que se recibió asistencia de IA
ChatGPT fue utilizado principalmente en los siguientes aspectos:
Relaciones entre entidades: orientación para establecer las relaciones entre Personaje, CardPersonaje, Evento y EstadisticaPersonaje, así como para configurar Entity Framework Core y su contexto.
Migraciones: apoyo para preparar y aplicar la segunda migración, relacionada con la incorporación de las estadísticas de personajes. La migración inicial fue realizada sin asistencia de IA.
Servicios y lógica de negocio: apoyo en la revisión y adaptación de servicios, especialmente en las reglas relacionadas con las simulaciones de eventos, la participación de personajes, los fallecimientos y la actualización de estadísticas.
Simulaciones: asistencia para revisar el funcionamiento de las simulaciones de enfrentamientos, el cálculo de resultados y la actualización de los datos relacionados con los personajes.
Documentación de Swagger: apoyo para redactar descripciones de endpoints, documentar respuestas HTTP y mejorar la información presentada en Swagger.
Revisión y depuración: ayuda para interpretar errores, analizar posibles soluciones y revisar implementaciones durante el desarrollo.
Participación y validación
Las sugerencias de la inteligencia artificial no se incorporaron automáticamente. Las propuestas y el código fueron revisados durante el desarrollo, realizando observaciones, solicitando modificaciones y descartando alternativas cuando no se ajustaban a los requisitos o decisiones del proyecto.
También se realizaron pruebas de compilación, ejecución, migraciones y funcionamiento de los endpoints mediante Swagger para verificar las implementaciones.
Declaración final
La inteligencia artificial fue utilizada como herramienta de apoyo durante distintas etapas del desarrollo, principalmente para el análisis de relaciones entre entidades, la implementación de determinadas reglas de negocio, las simulaciones, las estadísticas y la documentación.
El desarrollo incluyó revisión, adaptación y validación de las soluciones propuestas. La responsabilidad sobre el código entregado y su explicación corresponde al estudiante.


