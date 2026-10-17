# Ejercicios de patrones

## Estado de esta rama

En el árbol de código actual, los cinco ejercicios tienen implementación en `src/LibraryLab.Application/Exercises/PatternExerciseContracts.cs`, y sus cinco pruebas en `LibraryLab.Patterns.Tests` pasan. Esto difiere de la plantilla inicial, en la que los contratos estaban pendientes. No se deben modificar ni debilitar las pruebas para alterar ese resultado.

La tabla resume el alcance observable de cada prueba, no instrucciones de implementación.

## Creacionales

| Patrón | Problema del escenario | Comportamiento esperado | Prueba |
|---|---|---|---|
| Factory Method | El ejercicio debe producir un préstamo que represente el comando y el tipo de recurso solicitados. | Conserva usuario, recurso y tipo de recurso en el préstamo resultante. | `FactoryMethodExerciseTests.Creates_a_loan_with_the_requested_resource_type` |
| Builder | El recibo de préstamo requiere los datos que serán presentados después del registro. | Conserva identificador del préstamo, recurso y fecha límite recibidos. | `BuilderExerciseTests.Builds_a_receipt_that_preserves_loan_details` |

## Estructurales

| Patrón | Problema del escenario | Comportamiento esperado | Prueba |
|---|---|---|---|
| Adapter | El catálogo antiguo entrega registros con un contrato distinto al de importación de LibraryLab. | El resultado conserva código, nombre y categoría, y cuenta con identificador. | `AdapterExerciseTests.Converts_a_legacy_catalog_record_to_the_library_import_contract` |
| Decorator | El envío de avisos de préstamo requiere registro de auditoría adicional. | Entrega el mismo aviso al emisor y al registro de auditoría. | `DecoratorExerciseTests.Sends_the_loan_notice_to_the_configured_sender` |
| Facade | Consultar el resumen de un miembro requiere datos del miembro, de sus préstamos y de los recursos relacionados. | Devuelve nombre de usuario y recibos de sus préstamos activos. | `FacadeExerciseTests.Produces_a_member_overview_with_active_loan_information` |

## Protección académica

Las reglas académicas y los límites de asistencia están en `AGENTS.md`. Si se trabaja sobre una copia de la plantilla con contratos pendientes, las pruebas de patrones se ejecutan por separado y sus fallos no deben ocultarse ni corregirse modificando las pruebas. Confirma que la implementación de esta rama está autorizada para tu entrega.
