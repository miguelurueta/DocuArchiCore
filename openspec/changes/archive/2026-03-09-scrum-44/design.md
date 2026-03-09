## Context

- Change key: `scrum-44`
- Objetivo: implementar servicio `ValidaCamposObligatorios` para radicación entrante.
- Ubicación principal: `MiApp.Services/Service/Radicacion/Tramite`.

## Problem Statement

La validación de obligatorios está distribuida y no existe un servicio único que combine campos fijos y dinámicos de plantilla. Esto aumenta acoplamiento, dificulta pruebas y genera riesgo de inconsistencias funcionales.

## Goals

- Centralizar validación de obligatorios en un servicio reusable.
- Unificar campos fijos obligatorios en una constante compartida.
- Determinar obligatorios dinámicos con `DetallePlantillaRadicado` (sin leer tabla física de plantilla).
- Retornar `AppResponses<List<ValidationError>>` usando `ValidationHelper`.
- Cumplir patrón `Service -> Repository`, SoC, SOLID y pruebas unitarias/integración.

## Non-Goals

- Cambiar contratos existentes no relacionados con validación de obligatorios.
- Agregar reglas de negocio fuera del proceso de validación de campos.

## Proposed Architecture

1. Service Layer (`MiApp.Services`)
- Nuevo servicio `ValidaCamposObligatoriosService`.
- Interfaz en el mismo archivo para facilitar test unitario.
- Método principal recibe:
  - `RegistrarRadicacionEntranteRequestDto request`
  - `string defaultDbAlias`
  - `IReadOnlyCollection<DetallePlantillaRadicado> detallePlantilla`
- Construye lista de faltantes (fijos + dinámicos) y retorna `AppResponses<List<ValidationError>>`.
- Envoltura `try/catch` con error controlado.

2. Repository Layer (`MiApp.Repository`)
- Reuso de repositorio existente para obtener `DetallePlantillaRadicado` (consultas parametrizadas).
- Sin validación de negocio en repositorio.

3. API Layer (`DocuArchi.Api`)
- Registrar interfaz/implementación en `Program.cs`.
- Invocar validación desde el flujo de radicación entrante.

4. Shared Constants
- Crear catálogo central de campos fijos requeridos para reutilización transversal.

## Fixed Required Fields Baseline

Campos fijos con obligación declarada en ticket (ejemplos marcados como requeridos):
- `Destinatario_Externo_id_Dest_Ext` requerido
- `Remit_Dest_Interno_id_Remit_Dest_Int` requerido
- `Usuario_Radicador_id_usuario` requerido
- `System_Plantilla_Radicado_id_Plantilla` requerido
- `Consecutivo_Rad` requerido
- `Consecutivo_CodBarra` requerido
- `Fecha_Radicado` requerido
- `Descripcion_Documento` requerido
- `Numero_Folios` requerido
- `Destinatario_Cor` requerido
- `Remitente_Cor` requerido
- `Anexos_Cor` requerido
- `Asunto` requerido
- `Codigo_Sede` requerido
- `FECHALIMITERESPUESTA` requerido
- `Id_area_remit_dest_interno` requerido
- `Area_remit_dest_interno` requerido
- `CARGO_DESTINATARIO` requerido
- `Tipo_radicado_plantilla` requerido
- `TipoRadicado` requerido
- `IdtipoRadicado` requerido
- `IdTipoRadicado` requerido

## Response Contract Decisions

- Si hay errores: `Success=false`, `Data=List<ValidationError>`, `Message` descriptivo.
- Si no hay errores:
  - Caso con coincidencias válidas: `Success=true`, `Data=List<ValidationError>` vacía.
  - Caso "sin resultados" definido por ticket: `Success=true`, `Data=null`, `Message=\"Sin resultados\"`.
- En excepción: `Success=false`, `errors = new[] { new AppError { Field = "ValidaCamposObligatorios", Message = ex.Message, Type = "Technical" } }`, `Data` vacío.

## Testing Strategy

- Unit tests:
  - éxito con datos válidos
  - sin resultados
  - excepción simulada
- Integration tests MySQL con Testcontainers:
  - validación real con campos dinámicos de plantilla
  - consultas parametrizadas
- Contract tests de estructura `AppResponses`.

## Documentation Scope

- Diagramas: casos de uso, clases, secuencia, estado en `DocuArchiCore/Docs/Radicacion/Tramite`.
- Documento frontend: descripción DTO y comportamiento de respuesta.
