# SCRUM-154 — Arquitectura: Resolve Editor Document

## Propósito

Centralizar en backend la resolución de contenido del editor para que el frontend haga una sola llamada:

- Si existe documento → retorna `Mode=existing`.
- Si no existe documento → retorna `Mode=initial` (HTML inicial).
- Si `prefer=initial` → fuerza `Mode=initial` cuando el contexto lo permite.

## Componentes reales (código)

- Controller: `..\DocuArchi.Api\Controllers\GestorDocumental\Editor\ResolveEditorDocumentController.cs`
- Service: `..\MiApp.Services\Service\GestorDocumental\Editor\ServiceResolveEditorDocument.cs`
- DTO: `..\MiApp.DTOs\DTOs\GestorDocumental\Editor\EditorResolveDocumentResponseDto.cs`

## Diagrama de Estado

```mermaid
stateDiagram-v2
  [*] --> Validando
  Validando --> Rechazado: claim/parámetros inválidos
  Validando --> ConsultandoContexto: OK
  ConsultandoContexto --> ContandoDocumentos
  ContandoDocumentos --> Conflicto: count > 1
  ContandoDocumentos --> ConsultandoExistente: count == 1 & prefer=existing
  ContandoDocumentos --> ValidandoPreferInitial: count == 1 & prefer=initial
  ValidandoPreferInitial --> Conflicto: no permite múltiples
  ValidandoPreferInitial --> GenerandoInitial: permite múltiples
  ContandoDocumentos --> GenerandoInitial: count == 0
  ConsultandoExistente --> Exitoso
  GenerandoInitial --> Exitoso
  Conflicto --> [*]
  Rechazado --> [*]
  Exitoso --> [*]
```

## Diagrama de Secuencia

```mermaid
sequenceDiagram
  autonumber
  participant FE as Frontend
  participant C as ResolveEditorDocumentController
  participant S as ServiceResolveEditorDocument
  participant CC as ISolicitaEditorContextDefinitionRepository
  participant DR as ISolicitaEditorDocumentByContextRepository
  participant DS as IServiceSolicitaEditorDocumentByContext
  participant IS as IServiceInitialContentEditor
  participant TR as ITemplateDefinitionsRepository
  participant DB as BD

  FE->>C: GET /document/resolve?contextCode&entityId&idTareaWf&prefer
  C->>C: Validar claim defaulalias + parámetros
  C->>S: ResolveAsync(...)

  S->>CC: SolicitaPorContextCodeAsync(contextCode)
  CC->>DB: SELECT ra_editor_context_definitions

  S->>DR: CountActiveByContextAsync(contextDefinitionId, entityId)
  DR->>DB: COUNT documentos activos por contexto+entidad

  alt count > 1
    S-->>C: Conflict
  else count == 1 && prefer=existing
    S->>DS: SolicitaEditorDocumentByContextAsync(contextCode, entityId)
    DS->>DB: SELECT documento existente
    S-->>C: Mode=existing
  else initial (count==0 o prefer=initial permitido)
    S->>IS: GetInitialContentAsync(idTareaWf, contextCode, entityId, overrides)
    IS->>DB: Consulta estructura + plantilla + tokens
    S->>TR: GetDefinitionByCodeAsync(templateCode)
    TR->>DB: SELECT ra_editor_template_definitions
    S-->>C: Mode=initial (html + tokensResueltos)
  end

  C-->>FE: 200 / 400 / 409
```

## Decisiones relevantes

- El controller solo valida claim/parámetros y delega.
- El service concentra reglas de negocio:
  - valida contexto activo,
  - detecta múltiples documentos (conflicto),
  - decide existing vs initial,
  - obliga `idTareaWf` únicamente cuando se requiere `initial`.

## Deuda técnica identificada (para roadmap)

- Cuando hay múltiples documentos (409), falta un flujo oficial para que el frontend liste/seleccione un documento.
- Actualmente la detección de “documento activo” depende de la implementación del repositorio `CountActiveByContextAsync`.

