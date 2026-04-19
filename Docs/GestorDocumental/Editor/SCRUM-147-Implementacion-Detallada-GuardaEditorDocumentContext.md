# SCRUM-147 — Implementación Detallada: GuardaEditorDocumentContext (Catálogo)

## Archivos (por capa)

- API
  - `DocuArchi.Api/Controllers/GestorDocumental/Editor/GuardaEditorDocumentContextController.cs`
  - `DocuArchi.Api/Program.cs` (DI)
- Services
  - `MiApp.Services/Service/GestorDocumental/Editor/ServiceGuardaEditorDocumentContext.cs`
- Repository
  - `MiApp.Repository/Repositorio/GestorDocumental/Editor/SolicitaEditorContextDefinitionRepository.cs`
  - `MiApp.Repository/Repositorio/GestorDocumental/Editor/GuardaEditorDocumentContextRepository.cs`
- DTO
  - `MiApp.DTOs/DTOs/GestorDocumental/Editor/GuardaEditorDocumentContextDto.cs`
- Models
  - `MiApp.Models/Models/GestorDocumental/Editor/RaEditorContextDefinition.cs`
  - `MiApp.Models/Models/GestorDocumental/Editor/RaEditorDocumentContext.cs`

## Contrato

Endpoint (misma API):

- `POST /api/gestor-documental/editor/document/context`

Request:

- `DocumentId`
- `ContextCode`
- `EntityId`
- `CreatedBy` (opcional)

## Reglas implementadas

### Validaciones

- `defaulalias` requerido (claim)
- `DocumentId > 0`
- `ContextCode` requerido y `Trim()`
- `EntityId > 0`

### Catálogo (control)

- `ContextCode` debe existir y estar `is_active = 1`

### Persistencia (idempotente)

- Si existe una relación activa equivalente (`document_id + context_definition_id + entity_id`), retorna la existente.
- Si no existe, inserta en `ra_editor_document_context` con:
  - `is_active = 1`
  - `created_at = UTC now`
  - `created_by` desde request

### Reglas de catálogo (mínimo)

- `requires_unique_entity`:
  - si existe un contexto activo para el mismo `context_definition_id + entity_id` pero con `document_id` distinto, retornar error de validación.
- `allow_multiple_contexts_per_document`:
  - si es `false` y ya existe contexto activo para `document_id + context_definition_id` con `entity_id` distinto, retornar error de validación.

## Uso como repositorio base dentro del Full Save

El repositorio `IGuardaEditorDocumentContextRepository` soporta:

- `IDbConnection? conn`
- `IDbTransaction? trans`

Esto permite que el Full Save orquestador ejecute “Guardar Documento + Guardar Contexto + Sincronizar Imágenes” en una única transacción atómica.

## Script SQL recomendado (referencia)

Notas:

- Ajustar tipo de PK a la convención de la BD.
- MySQL: usar `BIGINT`/`TINYINT(1)` y `DATETIME`.

```sql
CREATE TABLE IF NOT EXISTS ra_editor_context_definitions (
  context_definition_id BIGINT PRIMARY KEY AUTO_INCREMENT,
  context_code VARCHAR(100) NOT NULL,
  entity_name VARCHAR(100) NOT NULL,
  relation_type VARCHAR(100) NOT NULL,
  module_name VARCHAR(100) NULL,
  description VARCHAR(255) NULL,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  allow_multiple_documents TINYINT(1) NOT NULL DEFAULT 1,
  allow_multiple_contexts_per_document TINYINT(1) NOT NULL DEFAULT 1,
  requires_unique_entity TINYINT(1) NOT NULL DEFAULT 0,
  created_by VARCHAR(100) NULL,
  created_at DATETIME NULL,
  UNIQUE KEY uq_ra_editor_context_definitions_code (context_code)
);

CREATE TABLE IF NOT EXISTS ra_editor_document_context (
  context_id BIGINT PRIMARY KEY AUTO_INCREMENT,
  document_id BIGINT NOT NULL,
  context_definition_id BIGINT NOT NULL,
  entity_id BIGINT NOT NULL,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_by VARCHAR(100) NULL,
  created_at DATETIME NULL,
  deactivated_by VARCHAR(100) NULL,
  deactivated_at DATETIME NULL,
  KEY ix_doc_ctx_doc (document_id),
  KEY ix_doc_ctx_entity (context_definition_id, entity_id, is_active),
  KEY ix_doc_ctx_doc_def (document_id, context_definition_id, is_active)
);
```

