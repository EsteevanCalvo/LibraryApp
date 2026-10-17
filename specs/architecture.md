# Arquitectura

## Objetivo y límites

LibraryLab administra miembros, libros, equipos y préstamos mediante una API ASP.NET Core 8. La separación en proyectos permite estudiar el dominio sin acoplarlo al transporte HTTP ni a EF Core.

## Dirección de dependencias

`Api -> Application -> Domain <- Infrastructure`

- **Domain** contiene entidades, estados e invariantes. No referencia frameworks web ni persistencia.
- **Application** define casos de uso, contratos de repositorio y resultados de lectura. Solo depende de Domain.
- **Infrastructure** implementa repositorios, transacciones y mapeos EF Core/MySQL para los contratos de Application.
- **Api** convierte DTO HTTP en comandos de Application, registra dependencias y traduce errores a respuestas HTTP.

La API delega reglas de préstamo y devolución a Application y Domain. El control de concurrencia se apoya en una versión de disponibilidad persistida como token de concurrencia y en una transacción.

## Proyectos

`src/LibraryLab.Domain`, `src/LibraryLab.Application`, `src/LibraryLab.Infrastructure` y `src/LibraryLab.Api` forman la solución. Los proyectos `tests/LibraryLab.*.Tests` separan pruebas normales, de arquitectura y ejercicios académicos.

## Ejercicios académicos

En esta rama, Factory Method, Builder, Adapter, Decorator y Facade están implementados en Application. Este estado puede diferir de la plantilla inicial; el alcance permitido para una entrega académica se rige por `AGENTS.md`.
