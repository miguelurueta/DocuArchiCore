# PROMPT ARQUITECTONICO - Ticket 07 BE

# Crear endpoint de autocomplete para Workflow Inbox

Rol esperado:

Arquitecto de software senior backend (.NET, C#, APIs REST, seguridad, performance, contracts DTO, query builders).

## OBJETIVO

Crear un endpoint backend dedicado para autocomplete de tareas workflow de gestion de correspondencia, retornando sugerencias limitadas, seguras y desacopladas de la estructura completa de la bandeja, sin reutilizar el endpoint paginado de listado.

El endpoint debe reutilizar el mismo contexto de seguridad y resolucion de metadata de Workflow Inbox, pero con una semantica optimizada para sugerencias.

Este ticket es continuacion de:

`Docs/Architecture/GestionCorrespondencia/01-EB Normalizar-SearchType-like-workflow-inbox.md`

`Docs/Architecture/GestionCorrespondencia/02-BE-Endurecer-like-seguridad-rendimiento-workflow-inbox.md`

## CONTEXTO EXISTENTE

Componentes existentes:

Controller:

`DocuArchi.Api/Controllers/WorkflowInboxGestion/WorkflowInboxController.cs`

Service:

`MiApp.Services/Service/Workflow/BandejaCorrespondencia/WorkflowInboxService.cs`

Repository:

`IWorkflowInboxRepository` y `WorkflowInboxRepository` existen actualmente dentro de:

`MiApp.Services/Service/Workflow/BandejaCorrespondencia/WorkflowInboxService.cs`

No asumir que existe un archivo separado `WorkflowInboxRepository.cs`.

QueryBuilder:

`MiApp.Services/Service/Workflow/BandejaCorrespondencia/WorkflowInboxQueryBuilder.cs`

DTO de metadata dinamica real:

`MiApp.DTOs/DTOs/Workflow/BandejaCorrespondencia/WorkflowDynamicColumnDefinitionDto.cs`

DTO request publico actual:

`MiApp.DTOs/DTOs/Workflow/BandejaCorrespondencia/WorkflowInboxApiRequestDto.cs`

Tests backend relacionados:

`DocuArchiCore/tests/TramiteDiasVencimiento.Tests/WorkflowInboxControllerTests.cs`

`DocuArchiCore/tests/TramiteDiasVencimiento.Tests/WorkflowInboxServiceTests.cs`

`DocuArchiCore/tests/TramiteDiasVencimiento.Tests/WorkflowInboxQueryBuilderTests.cs`

## UBICACION ESPERADA

Revisar o ajustar unicamente si aplica:

`DocuArchi.Api/Controllers/WorkflowInboxGestion/WorkflowInboxController.cs`

`MiApp.Services/Service/Workflow/BandejaCorrespondencia/WorkflowInboxService.cs`

`MiApp.Services/Service/Workflow/BandejaCorrespondencia/WorkflowInboxQueryBuilder.cs`

DTOs nuevos de request/response para autocomplete en:

`MiApp.DTOs/DTOs/Workflow/BandejaCorrespondencia/`

Tests backend relacionados en:

`DocuArchiCore/tests/TramiteDiasVencimiento.Tests/`

No separar `WorkflowInboxRepository` a un archivo nuevo salvo que exista justificacion tecnica clara. El alcance principal es funcional, no reorganizacion de archivos.

## RUTA OFICIAL

Agregar accion nueva en el controller actual:

`POST /api/workflowInboxgestion/inboxgestion/autocomplete`

Con la ruta actual del controller, esto corresponde a:

```csharp
[HttpPost("inboxgestion/autocomplete")]
```

## CONTRATO OFICIAL

Request:

```csharp
public sealed class WorkflowInboxAutocompleteRequestDto
{
    public string? Search { get; set; }
    public int? Limit { get; set; }
}
```

Response:

```csharp
public sealed class WorkflowInboxAutocompleteResponseDto
{
    public IReadOnlyList<WorkflowInboxAutocompleteItemDto> Items { get; set; } = [];
}

public sealed class WorkflowInboxAutocompleteItemDto
{
    public string Value { get; set; } = string.Empty;
    public string? Label { get; set; }
    public string? Field { get; set; }
}
```

Respuesta del endpoint:

`AppResponses<WorkflowInboxAutocompleteResponseDto>`

## SEMANTICA DEL RESPONSE

- `Value`: representa el valor textual sugerido.
- `Label`: representa el texto visible para UI. Si no existe formato adicional, puede ser igual a `Value`.
- `Field`: representa el identificador logico de la columna origen resuelta desde metadata backend, por ejemplo `WorkflowDynamicColumnDefinitionDto.Key`.

No retornar filas completas de tareas workflow.

## RESTRICCIONES OBLIGATORIAS

- No retornar filas completas de tareas workflow.
- No exponer campos no visibles o no filtrables.
- No omitir validacion de claims.
- No permitir `Limit` sin maximo.
- No consultar si `Search` es menor al minimo definido.
- No duplicar logica de resolucion de contexto workflow.
- No reemplazar el endpoint de listado paginado.
- No introducir SQL libre inseguro.
- No permitir columnas enviadas por el cliente.
- No reutilizar `SearchType = 2` global contains para autocomplete.
- No modificar `DapperCrudEngine` salvo que exista una ampliacion explicita del alcance.

## COLUMNAS ELEGIBLES

Las columnas elegibles deben resolverse exclusivamente desde la metadata segura ya existente del workflow inbox en backend:

`List<WorkflowDynamicColumnDefinitionDto> dynamicColumns`

Solo incluir columnas que cumplan:

- `IsVisible = true`
- `IsFilterable = true`
- `DataType = "text"` segun `WorkflowInboxQueryPolicy.IsTextDataType(...)`
- metadata valida segun `WorkflowInboxQueryPolicy.IsSelectableColumn(...)`

No asumir existencia de `WorkflowInboxResolvedMetadata`, `EsVisible`, `EsBuscable`, `EsFiltrable` o `TipoDato`.

No aceptar columnas desde el request.

## REGLAS DE IMPLEMENTACION

1. Agregar accion nueva en `WorkflowInboxController`.
2. Reutilizar o extraer validacion comun de claims del endpoint existente:
   - `defaulalias`
   - `usuarioid`
3. Mantener obligatoria la validacion de `usuarioid` como entero.
4. Agregar metodo nuevo en `IWorkflowInboxService`, por ejemplo:
   - `AutocompleteBandejaWorkflowAsync(WorkflowInboxAutocompleteRequestDto request, int idUsuarioGestion, string defaultDbAlias)`
5. Reutilizar `IWorkflowInboxContextResolverService.ResolveAsync(idUsuarioGestion)`.
6. Reutilizar `IWorkflowRouteColumnConfigRepository.GetColumnsByRouteAsync(...)` con la ruta y modo aplicables.
7. Crear DTOs especificos para autocomplete.
8. Aplicar `minLength` backend fijo o configurable:
   - recomendado: `3`
9. Aplicar `limit` maximo backend fijo o configurable:
   - recomendado: `10`
10. Buscar solo en columnas textuales visibles y filtrables.
11. Retornar sugerencias distintas.
12. Ordenar sugerencias de forma estable.
13. Retornar `AppResponses<WorkflowInboxAutocompleteResponseDto>`.

## ESTRATEGIA DE BUSQUEDA

La busqueda del autocomplete debe usar busqueda por prefijo:

```sql
LIKE 'valor%'
```

No usar por defecto:

```sql
LIKE '%valor%'
```

Justificacion:

- Mejor performance.
- Mejor UX para autocomplete.
- Menor costo por tecla.

Si en el futuro se requiere contains, debe ser una evolucion explicita y documentada, no parte automatica de este ticket.

La normalizacion y escape del termino debe reutilizar la politica endurecida del Ticket 06:

- `Trim()`.
- `minLength`.
- `maxLength`.
- escape de comilla simple.
- escape de caracteres especiales de LIKE: `%` y `_`.
- no permitir comodines controlados por cliente.

## ESTRATEGIA DE QUERY COMPATIBLE CON EL REPO

El autocomplete multi-columna no debe forzar un refactor grande de `DapperCrudEngine`.

Opcion recomendada:

1. El service resuelve contexto y metadata igual que el inbox.
2. El service filtra columnas elegibles o delega ese filtrado al QueryBuilder.
3. El repository solicita al QueryBuilder una consulta por columna elegible, por ejemplo:

```sql
SELECT DISTINCT DAT.asunto AS Value, 'asunto' AS Field
FROM dat_adic_tar{Ruta} DAT
INNER JOIN estados_tarea_workflow etw ON etw.Inicio_Tareas_Workflow_id_Tarea = DAT.INICIO_TAREAS_WORKFLOW_ID_TAREA
WHERE (FiltrosBase)
AND DAT.asunto LIKE 'abc%'
LIMIT @limit
```

4. El repository/service fusiona resultados.
5. Elimina duplicados.
6. Ordena de forma estable.
7. Recorta al limite efectivo.

No usar `UNION` dinamico salvo que se justifique y se pruebe con el motor real.

## REGLAS DE EJECUCION

- Aplicar `Trim()` a `Search` antes de evaluar.
- Si `Search` es null, vacio o whitespace, retornar `success = true` con `Items = []`.
- Si `Search.Length < minLength`, retornar `success = true` con `Items = []`.
- Si `Limit` no viene o es menor/igual a cero, usar default backend.
- Si `Limit > maximo permitido`, recortar al maximo.
- Si no hay columnas elegibles, retornar `success = true` con `Items = []`.
- Nunca lanzar error funcional solo por ausencia de resultados.

## REGLAS DE SEGURIDAD

- Respetar claims existentes del modulo.
- Reutilizar validacion actual de usuario autenticado.
- Respetar `defaultAlias` y contexto workflow resuelto.
- Usar `defaulaliaswf` para consultas de metadata/datos workflow igual que el flujo existente.
- No devolver mas informacion de la que el usuario puede consultar.
- No exponer columnas fuera de metadata segura.
- No construir SQL con nombres de columnas provenientes del cliente.
- No saltar filtros base existentes de workflow inbox salvo decision explicita documentada.

## REGLAS DE PERFORMANCE

- No consultar si `Search` no supera `minLength`.
- Limitar resultados con limite maximo estricto.
- No devolver filas completas.
- No ejecutar logica de tabla paginada completa.
- Mantener costo de consulta acotado para escenarios de tipeo frecuente.
- Usar solo columnas elegibles necesarias.
- Evitar contains global por defecto.
- Evitar `CAST` sobre fechas/numeros.

## RIESGOS A EVITAR

- Usar autocomplete como exportacion accidental.
- Filtrar informacion de campos no visibles.
- Duplicar reglas de claims.
- No limitar resultados.
- Consultas pesadas por cada tecla.
- Acoplar response a estructura interna de tabla dinamica.
- Usar contains global por defecto.
- Devolver datos inconsistentes con metadata segura.
- Reabrir o contradecir reglas de SearchType definidas en Tickets 05 y 06.

## PRUEBAS UNITARIAS OBLIGATORIAS

Controller:

- Endpoint valida claim `defaulalias`.
- Endpoint valida claim `usuarioid`.
- Endpoint rechaza `usuarioid` invalido.
- Endpoint llama al service con `WorkflowInboxAutocompleteRequestDto`, `idUsuarioGestion` y `defaultDbAlias`.
- Endpoint retorna `AppResponses<WorkflowInboxAutocompleteResponseDto>`.

Service:

- Request menor a `minLength` retorna `Items = []` sin consultar repository final.
- `Search` null/vacio/whitespace retorna `Items = []`.
- `Limit` mayor al maximo se recorta.
- Sin columnas elegibles retorna `Items = []`.
- Reutiliza context resolver.
- Reutiliza metadata repository.
- Propaga errores controlados de contexto y metadata.

QueryBuilder / Repository:

- Campos no visibles no generan sugerencias.
- Campos no filtrables no generan sugerencias.
- Campos no textuales no generan sugerencias.
- Query usa prefijo `LIKE 'valor%'`.
- Query no usa contains `LIKE '%valor%'`.
- Query aplica escape del Ticket 06.
- Query no genera SQL invalido sin columnas elegibles.
- Sugerencias se retornan sin duplicados.
- `Field` corresponde a columna logica valida.
- `Value` no es vacio.
- `Label` puede ser igual a `Value`.

## PRUEBAS DE INTEGRACION / CALIDAD

- Endpoint responde con `AppResponses<WorkflowInboxAutocompleteResponseDto>`.
- Endpoint respeta claims requeridos.
- Endpoint no devuelve filas completas.
- Endpoint respeta limite maximo.
- Endpoint usa metadata segura real.
- Endpoint mantiene comportamiento estable con metadata representativa.
- Endpoint nunca retorna mas elementos que el limite efectivo.
- Endpoint no rompe al no existir resultados.
- Busqueda con caracteres especiales no rompe consulta.

## CRITERIOS DE ACEPTACION

- Existe endpoint dedicado de autocomplete.
- Autocomplete usa el mismo contexto y claims del workflow inbox.
- Response contiene sugerencias limitadas y seguras.
- No se reutiliza el endpoint paginado para sugerencias.
- Las columnas elegibles se resuelven desde metadata segura real.
- La busqueda usa prefijo por defecto.
- Las respuestas vacias retornan `Items = []`.
- No se aceptan columnas desde cliente.
- No se exponen campos no visibles, no filtrables o no textuales.
- Pruebas backend cubren contrato, seguridad, limites y bordes.
