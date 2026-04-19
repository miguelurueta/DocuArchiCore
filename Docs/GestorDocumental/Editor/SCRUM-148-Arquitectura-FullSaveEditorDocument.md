# SCRUM-148 — Arquitectura: Full Save Editor Document (Catálogo)

## Propósito

El Full Save es el **service orquestador** que garantiza atomicidad (una sola transacción) entre:

1) Guardar documento HTML  
2) Resolver catálogo de contexto (`ContextCode`)  
3) Guardar relación documento ↔ contexto  
4) Sincronizar imágenes

## Diagrama de Estado

```mermaid
stateDiagram-v2
  [*] --> Validando
  Validando --> AbriendoTransaccion: Request OK
  Validando --> Rechazado: Validación/Claim inválido

  AbriendoTransaccion --> GuardandoDocumento
  GuardandoDocumento --> ResolviendoCatalogo: DocumentId OK
  GuardandoDocumento --> Rollback: Falla

  ResolviendoCatalogo --> GuardandoContexto: ContextCode activo
  ResolviendoCatalogo --> Rollback: ContextCode inválido/inactivo

  GuardandoContexto --> SincronizandoImagenes: Contexto OK
  GuardandoContexto --> Rollback: Falla

  SincronizandoImagenes --> Commit: Sync OK
  SincronizandoImagenes --> Rollback: Falla

  Commit --> Exitoso
  Rollback --> Fallido

  Exitoso --> [*]
  Rechazado --> [*]
  Fallido --> [*]
```

## Diagrama de Casos de Uso

```mermaid
flowchart LR
  A[Frontend / Consumidor] -->|POST /document/full-save| UC1((Full Save Documento))
  UC1 --> UC2((Guardar/Actualizar Documento))
  UC1 --> UC3((Resolver Catálogo Contexto))
  UC1 --> UC4((Guardar Contexto Documento))
  UC1 --> UC5((Sincronizar Imágenes))
  UC1 --> UC6((Rollback/Commit Atómico))
```

## Capas

Controller → Service (orquestador) → Repositorios → BD

### Controller

- Valida claim `defaulalias`
- Valida request y campos mínimos
- Delegación al service

### Service Full Save (orquestador)

- Abre `IDbConnection` y `IDbTransaction`
- Ejecuta repositorios reutilizados pasando `conn/trans`
- Ejecuta `COMMIT` solo si todo OK
- Ejecuta `ROLLBACK` ante cualquier error o excepción

## Repositorios reutilizados

- `IGuardaEditorDocumentRepository` (documento)
- `ISolicitaEditorContextDefinitionRepository` (catálogo)
- `IGuardaEditorDocumentContextRepository` (contexto)
- `ISincronizaEditorDocumentImagesRepository` (imágenes)

## Diagrama de Secuencia

```mermaid
sequenceDiagram
  autonumber
  participant FE as Frontend
  participant C as FullSaveEditorDocumentController
  participant S as ServiceFullSaveEditorDocument
  participant RD as IGuardaEditorDocumentRepository
  participant RC as ISolicitaEditorContextDefinitionRepository
  participant RX as IGuardaEditorDocumentContextRepository
  participant RI as ISincronizaEditorDocumentImagesRepository
  participant DB as BD

  FE->>C: POST /document/full-save (DocumentHtml, ContextCode, EntityId, ImageUids)
  C->>C: Validar claim defaulalias + request
  C->>S: FullSaveAsync(request, defaulalias)
  S->>DB: BEGIN TRANSACTION
  S->>RD: GuardaEditorDocumentAsync(..., conn, trans)
  RD->>DB: INSERT/UPDATE ra_editor_documents
  RD-->>S: AppResponses<RaEditorDocument> (DocumentId)
  S->>RC: SolicitaPorContextCodeAsync(ContextCode, conn, trans)
  RC->>DB: SELECT ra_editor_context_definitions (is_active=1)
  RC-->>S: AppResponses<RaEditorContextDefinition>
  S->>RX: GuardaEditorDocumentContextAsync(documentId, context_definition_id, entityId, ..., conn, trans)
  RX->>DB: INSERT/SELECT ra_editor_document_context (idempotente)
  RX-->>S: AppResponses<RaEditorDocumentContext>
  S->>RI: SincronizaAsync(documentId, imageUids, conn, trans)
  RI->>DB: UPDATE/DELETE/INSERT links
  RI-->>S: AppResponses<bool>
  S->>DB: COMMIT
  S-->>C: AppResponses<RaEditorDocument>
  C-->>FE: 200 OK
```

## Diagrama de Clases

```mermaid
classDiagram
  class FullSaveEditorDocumentController {
    +FullSave(FullSaveEditorDocumentRequestDto) ActionResult
  }

  class ServiceFullSaveEditorDocument {
    +FullSaveAsync(FullSaveEditorDocumentRequestDto,string) Task~AppResponses~RaEditorDocument~~
  }

  class IGuardaEditorDocumentRepository {
    +GuardaEditorDocumentAsync(GuardaEditorDocumentRequestDto,string,IDbConnection,IDbTransaction) Task~AppResponses~RaEditorDocument~~
  }
  class ISolicitaEditorContextDefinitionRepository {
    +SolicitaPorContextCodeAsync(string,string,IDbConnection,IDbTransaction) Task~AppResponses~RaEditorContextDefinition~~
  }
  class IGuardaEditorDocumentContextRepository {
    +GuardaEditorDocumentContextAsync(long,long,long,string,string,IDbConnection,IDbTransaction) Task~AppResponses~RaEditorDocumentContext~~
  }
  class ISincronizaEditorDocumentImagesRepository {
    +SincronizaAsync(long,IReadOnlyCollection~string~,string,IDbConnection,IDbTransaction) Task~AppResponses~bool~~
  }

  FullSaveEditorDocumentController --> ServiceFullSaveEditorDocument
  ServiceFullSaveEditorDocument --> IGuardaEditorDocumentRepository
  ServiceFullSaveEditorDocument --> ISolicitaEditorContextDefinitionRepository
  ServiceFullSaveEditorDocument --> IGuardaEditorDocumentContextRepository
  ServiceFullSaveEditorDocument --> ISincronizaEditorDocumentImagesRepository
```

## Invariantes

- No debe persistir documento sin contexto cuando el request exige contexto.
- No debe persistir documento/contexto si falla sincronización de imágenes.
- No debe haber commit parcial.
