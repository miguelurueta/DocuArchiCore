# SCRUM-149 — Arquitectura: SolicitaEditorDocumentByContext (Catálogo)

## Propósito

Recuperar un documento del editor Tiptap por un **contexto de negocio controlado por catálogo**, usando `ContextCode + EntityId`, sin depender de que el frontend conozca `DocumentId`.

## Tablas involucradas

- `ra_editor_context_definitions` (catálogo)
- `ra_editor_document_context` (relación activa)
- `ra_editor_documents` (documento)
- `ra_editor_document_image_links` (links)
- `ra_editor_document_images` (imágenes)

## Diagrama de Estado

```mermaid
stateDiagram-v2
  [*] --> Validando
  Validando --> Catalogo: Parámetros OK
  Validando --> Rechazado: Validación/Claim inválido

  Catalogo --> BuscandoContexto: ContextCode activo
  Catalogo --> Rechazado: ContextCode inválido/inactivo

  BuscandoContexto --> SinResultados: No existe relación activa
  BuscandoContexto --> ConsultandoDocumento: Contexto encontrado

  ConsultandoDocumento --> Inconsistencia: DocumentId no existe
  ConsultandoDocumento --> ConsultandoImagenes: Documento encontrado

  ConsultandoImagenes --> Exitoso

  SinResultados --> [*]
  Exitoso --> [*]
  Rechazado --> [*]
  Inconsistencia --> [*]
```

## Diagrama de Casos de Uso

```mermaid
flowchart LR
  FE[Frontend/Consumidor] --> UC1((Solicitar documento por contexto))
  UC1 --> UC2((Resolver catálogo por ContextCode))
  UC1 --> UC3((Buscar relación activa contexto))
  UC1 --> UC4((Consultar documento))
  UC1 --> UC5((Consultar imágenes activas))
```

## Diagrama de Secuencia

```mermaid
sequenceDiagram
  autonumber
  participant FE as Frontend
  participant C as SolicitaEditorDocumentByContextController
  participant S as ServiceSolicitaEditorDocumentByContext
  participant RC as ISolicitaEditorContextDefinitionRepository
  participant R as ISolicitaEditorDocumentByContextRepository
  participant DB as BD

  FE->>C: GET /document/by-context?contextCode&entityId
  C->>C: Validar claim defaulalias + parámetros
  C->>S: SolicitaEditorDocumentByContextAsync(contextCode, entityId, alias)
  S->>S: Normaliza contextCode (Trim + Upper)
  S->>RC: SolicitaPorContextCodeAsync(contextCode)
  RC->>DB: SELECT ra_editor_context_definitions (is_active=1)
  RC-->>S: context_definition_id + entity_name + relation_type
  S->>R: SolicitaEditorDocumentByContextAsync(context_definition_id, entityId)
  R->>DB: SELECT ra_editor_document_context (is_active=1) ORDER BY created_at DESC
  alt Sin resultados
    R-->>S: success=true, data=null
  else Contexto encontrado
    R->>DB: SELECT ra_editor_documents por document_id
    R->>DB: SELECT imágenes activas (deleted_at IS NULL)
    R-->>S: Documento + Imágenes + ContextId/EntityId
    S-->>C: Enriquecer Context (ContextCode, EntityName, RelationType)
  end
  C-->>FE: 200 OK / 400 BadRequest
```

## Secuencia literal (paso a paso)

1. El Controller valida `defaulalias`, `contextCode` y `entityId`.
2. El Service normaliza `contextCode` (`Trim().ToUpperInvariant()`).
3. El Service consulta el catálogo y valida que el `ContextCode` esté activo.
4. Con `context_definition_id`, el repositorio busca la relación activa más reciente para `entity_id`.
5. Si no existe relación activa: retorna `success=true` con `data=null`.
6. Si existe relación: consulta el documento por `document_id`.
7. Consulta imágenes asociadas activas (`deleted_at IS NULL`) y arma la lista `Images`.
8. El Service enriquece `Context` con datos del catálogo (`EntityName`, `RelationType`, `ContextCode`).

## Diagrama de Clases

```mermaid
classDiagram
  class SolicitaEditorDocumentByContextController {
    +GetByContext(contextCode, entityId) ActionResult
  }
  class ServiceSolicitaEditorDocumentByContext {
    +SolicitaEditorDocumentByContextAsync(contextCode, entityId, defaultDbAlias) Task~AppResponses~EditorDocumentDetailByContextResponseDto~~
  }
  class ISolicitaEditorContextDefinitionRepository {
    +SolicitaPorContextCodeAsync(contextCode, defaultDbAlias, conn, trans) Task~AppResponses~RaEditorContextDefinition~~
  }
  class ISolicitaEditorDocumentByContextRepository {
    +SolicitaEditorDocumentByContextAsync(contextDefinitionId, entityId, defaultDbAlias, conn, trans) Task~AppResponses~EditorDocumentDetailByContextResponseDto~~
  }

  SolicitaEditorDocumentByContextController --> ServiceSolicitaEditorDocumentByContext
  ServiceSolicitaEditorDocumentByContext --> ISolicitaEditorContextDefinitionRepository
  ServiceSolicitaEditorDocumentByContext --> ISolicitaEditorDocumentByContextRepository
```

## Diagrama de Tablas (relación lógica)

```mermaid
erDiagram
  ra_editor_context_definitions ||--o{ ra_editor_document_context : context_definition_id
  ra_editor_documents ||--o{ ra_editor_document_context : document_id
  ra_editor_documents ||--o{ ra_editor_document_image_links : document_id
  ra_editor_document_images ||--o{ ra_editor_document_image_links : image_id
```

## Decisión obligatoria: múltiples contextos activos

Si existen múltiples filas activas para el mismo `(context_definition_id, entity_id)`, se retorna la **más reciente**:

- `ORDER BY created_at DESC, context_id DESC LIMIT 1`

## No alcance

- No modifica datos.
- No expone `image_bytes`.
- No retorna contextos inactivos ni definiciones inactivas.

