## Context

- Jira issue key: SCRUM-207
- Jira summary: CREA-API-SOLICITA-GABINETE-RADICADO-IDTAREA-WORKFLOW
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-207

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

Se requiere exponer una API nueva para resolver metadatos de gabinete de documentos workflow con dos entradas:

- `consecutivoRadicado`
- `idTareaWorkflow`

La solución no debe modificar ni romper el endpoint existente `SolicitaExistenciaRadicadoRutaWorkflow`.

## Business Decisions

1. El sistema opera con una sola ruta workflow activa.
2. El backend resuelve internamente `Nombre_Ruta`; el frontend no envía `nombreRuta`.
3. La consulta principal se ejecuta sobre tabla dinámica `dat_adic_tar{Nombre_Ruta}`.
4. El nombre de gabinete se obtiene desde `configuracion_gabinete`.
5. Toda respuesta usa `AppResponses<T>` con semántica legacy `YES/NO`.

## Scope

### In Scope

- Endpoint GET por radicado:
  - `/api/workflow/ruta-trabajo/radicados/{consecutivoRadicado}/gabinete`
- Endpoint GET por tarea:
  - `/api/workflow/ruta-trabajo/tareas/{idTareaWorkflow}/gabinete`
- Servicio y repositorio nuevos para la consulta.
- DTO nuevo de salida.
- Registro DI.
- Pruebas unitarias/contrato mínimas de control.

### Out of Scope

- Cambiar contratos de `SolicitaExistenciaRadicadoRutaWorkflow`.
- Cambiar estructura de tablas workflow o gabinete.
- Crear endpoints de administración de rutas o gabinetes.

## Architecture

Patrón obligatorio:

- Controller -> Service -> Repository

Ubicación objetivo:

- Controller:
  - `DocuArchi.Api/Controllers/Radicacion/Tramite/RadicadoGabineteWorkflowController.cs`
- Service:
  - `MiApp.Services/Service/Workflow/RutaTrabajo/IRadicadoGabineteWorkflowService.cs`
  - `MiApp.Services/Service/Workflow/RutaTrabajo/RadicadoGabineteWorkflowService.cs`
- Repository:
  - `MiApp.Repository/Repositorio/Workflow/RutaTrabajo/IRadicadoGabineteWorkflowRepository.cs`
  - `MiApp.Repository/Repositorio/Workflow/RutaTrabajo/RadicadoGabineteWorkflowRepository.cs`
- DTO:
  - `MiApp.DTOs/DTOs/Workflow/RutaTrabajo/RadicadoGabineteWorkflowDto.cs`

## Reuse Strategy

Se reutiliza la resolución de ruta activa ya disponible:

- `ISolicitaEstructuraRutaWorkflowService`
- `SolicitaEstructuraRutaWorkflowAsync(defaultDbAliasWf)`

Regla:

- Tomar `FirstOrDefault()` de rutas activas y validar `Nombre_Ruta`.

## Data Access Rules

1. Usar `DapperCrudEngine` + `QueryOptions` en todos los accesos.
2. Validar `Nombre_Ruta` con regex:
   - `^[A-Za-z0-9_]+$`
3. Construir tabla dinámica solo después de validar regex:
   - `dat_adic_tar{Nombre_Ruta}`
4. No SQL manual concatenando valores de usuario.

## Alias Resolution

1. Workflow (`rutas_workflow` y `dat_adic_tar{ruta}`):
   - claim obligatorio `defaulaliaswf`
2. Gabinete (`configuracion_gabinete`):
   - usar `defaulaliaswf`
   - no depender de `defaulalias` para este caso de uso

## Response Contract

DTO:

```csharp
public sealed class RadicadoGabineteWorkflowDto
{
    public string Radicado { get; init; } = string.Empty;
    public long IdTareaWorkflow { get; init; }
    public long IdGabinete { get; init; }
    public string NombreGabinete { get; init; } = string.Empty;
    public string EstadoExistenciaRadicado { get; init; } = "NO";
}
```

Semántica:

- Encontrado: `success=true`, `message="YES"`, `EstadoExistenciaRadicado="YES"`.
- No encontrado: `success=true`, `message="YES"`, `EstadoExistenciaRadicado="NO"`.
- Si `EstadoExistenciaRadicado="YES"` y `NombreGabinete` vacío: `success=false`, `message="NombreGabinete requerido"`.
- Error validación/infraestructura: `success=false`, `errors[]` con `Field/Type/Message`.

## Flow

### Endpoint por Radicado

1. Validar `consecutivoRadicado`.
2. Obtener `defaulaliaswf`.
3. Resolver ruta activa (`Nombre_Ruta`).
4. Consultar `dat_adic_tar{Nombre_Ruta}` por `RADICADO`.
5. Si existe fila, tomar `ID_GABINETE` y consultar `configuracion_gabinete`.
6. Construir respuesta `AppResponses<RadicadoGabineteWorkflowDto>`.

### Endpoint por IdTareaWorkflow

1. Validar `idTareaWorkflow > 0`.
2. Obtener `defaulaliaswf`.
3. Resolver ruta activa (`Nombre_Ruta`).
4. Consultar `dat_adic_tar{Nombre_Ruta}` por `INICIO_TAREAS_WORKFLOW_ID_TAREA`.
5. Si existe fila, tomar `ID_GABINETE` y consultar `configuracion_gabinete`.
6. Construir respuesta `AppResponses<RadicadoGabineteWorkflowDto>`.

## Validation Rules

- `consecutivoRadicado` requerido y `Trim()`.
- `idTareaWorkflow` > 0.
- claim `defaulaliaswf` obligatorio.
- ruta activa obligatoria con `Nombre_Ruta` válido.
- casteos numéricos seguros para `ID_GABINETE`.

## Observability

Registrar logs estructurados:

- Inicio de solicitud.
- Ruta activa resuelta (`Nombre_Ruta`).
- Resultado de consulta dinámica (encontrado/no encontrado).
- Resolución de gabinete (id/nombre).
- Error técnico controlado.

Campos mínimos sugeridos:

- `requestId`, `consecutivoRadicado`, `idTareaWorkflow`, `nombreRuta`, `aliasWorkflow`, `aliasGabinete`.

## Risks and Mitigations

1. Riesgo: ruta activa no configurada.
   - Mitigación: retorno controlado de validación.
2. Riesgo: nombre de ruta inválido.
   - Mitigación: regex estricta antes de armar nombre de tabla.
3. Riesgo: gabinete en alias distinto.
   - Mitigación: unificar consulta en alias workflow (`defaulaliaswf`) para esta API.

## Acceptance Criteria

1. Existen los 2 endpoints nuevos y compilan.
2. Ambos resuelven ruta activa internamente sin `nombreRuta` en request.
3. Consulta a `dat_adic_tar{ruta}` es segura con regex y `QueryOptions`.
4. Se resuelve `Nombre_Gabinete` desde `configuracion_gabinete`.
5. No se altera el endpoint legacy existente.
6. Se agregan pruebas mínimas de éxito, no encontrado y validaciones.
