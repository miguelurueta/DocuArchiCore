# SCRUM-147 — Arquitectura: GuardaEditorDocumentContext (Catálogo)

## Propósito

Persistir la relación entre un documento del editor y un contexto de negocio de forma transversal y reutilizable, evitando strings libres y centralizando la definición del contexto en un catálogo.

## Tablas

### Catálogo

`ra_editor_context_definitions`

- `context_definition_id` (PK)
- `context_code` (único, requerido)
- `entity_name` (requerido)
- `relation_type` (requerido)
- `is_active`
- flags de reglas:
  - `allow_multiple_documents`
  - `allow_multiple_contexts_per_document`
  - `requires_unique_entity`

### Relación documento ↔ contexto

`ra_editor_document_context`

- `context_id` (PK)
- `document_id` (FK lógico a `ra_editor_documents`)
- `context_definition_id` (FK lógico a catálogo)
- `entity_id` (id real del negocio)
- `is_active`
- `created_by`, `created_at`
- `deactivated_by`, `deactivated_at`

## Capas

Controller → Service → (Repository catálogo + Repository contexto) → BD

### Controller

- Valida claim `defaulalias`
- Valida body (nulo) y campos mínimos
- Delegación al service

### Service (orquestación del caso de uso, no “Full Save”)

- Resuelve `ContextCode` → `context_definition_id` (catálogo)
- Aplica reglas del catálogo (mínimo: `requires_unique_entity`)
- Invoca repositorio de contexto para persistir (idempotente)

### Repository catálogo

- Consulta definición activa por `context_code` (parametrizado)

### Repository contexto

- Valida existencia del documento
- Idempotencia: no duplica relación activa equivalente
- Inserta y retorna fila persistida
- Soporta `IDbConnection? conn` / `IDbTransaction? trans` para transacción externa (Full Save)

## Secuencia (alto nivel)

1. Controller valida claim + request
2. Service valida datos
3. Service consulta catálogo por `ContextCode`
4. Service valida reglas del catálogo
5. Service registra relación (idempotente)
6. Retorna `AppResponses<RaEditorDocumentContext?>`

## No alcance

- No sincroniza imágenes
- No guarda/actualiza HTML
- No incorpora lógica específica de módulos (ej: `rad_gestion`)

