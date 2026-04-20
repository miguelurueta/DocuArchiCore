# SCRUM-154 — Arquitectura: ResolveEditorDocument

## Problema

El frontend debía decidir si llamar:
- `/initial-content` (plantilla + tokens) o
- `/document/by-context` / `/document/{documentId}` (documento guardado)

Esto duplicaba reglas de negocio y producía múltiples llamadas.

## Solución

Nuevo endpoint único:

`GET /api/gestor-documental/editor/document/resolve`

El backend decide:
- Si existe documento: `mode=existing`
- Si no existe documento: `mode=initial`

## Componentes

- API: `DocuArchi.Api.Controllers.GestorDocumental.Editor.ResolveEditorDocumentController`
- Service: `MiApp.Services.Service.GestorDocumental.Editor.ServiceResolveEditorDocument`
- Repositorio:
  - Lectura documento: `IServiceSolicitaEditorDocumentByContext` (ya existente)
  - Conteo para “múltiples”: `ISolicitaEditorDocumentByContextRepository.CountActiveByContextAsync`
  - Catálogo contexto: `ISolicitaEditorContextDefinitionRepository` (ya existente)
  - Catálogo plantillas: `ITemplateDefinitionsRepository` (ya existente)
  - Initial: `IServiceInitialContentEditor` (ya existente)

## Reglas clave

- `prefer=existing` (default): si hay documento, se devuelve.
- `prefer=initial`: intenta forzar inicial.
  - Si ya existe documento y el contexto no permite múltiples → `409 Conflict`.
- Múltiples documentos activos sin criterio determinístico → `409 Conflict`.

## Seguridad

- Controller valida claim `defaulalias`.
- Service recibe el alias validado y lo usa para repositorios/servicios.

## SOLID (resumen)

- SRP: controller solo HTTP/claim/validaciones básicas; service orquesta; repos hacen SQL.
- DIP: dependencias por interfaces.

