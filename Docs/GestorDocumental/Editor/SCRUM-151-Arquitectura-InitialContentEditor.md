# SCRUM-151 — Arquitectura: Initial Content Editor

## Propósito

Generar `htmlInicial` para Tiptap a partir de:

- estructura de negocio (por `idTareaWf`)
- `ContextCode` (catálogo de contextos)
- plantilla HTML (catálogo de plantillas + versiones)
- resolución de tokens y reemplazo en HTML

## Diagrama de Estado

```mermaid
stateDiagram-v2
  [*] --> Validando
  Validando --> ConsultandoEstructura: parámetros OK
  Validando --> Rechazado: validación/claim inválido
  ConsultandoEstructura --> ConsultandoCatalogoContexto
  ConsultandoCatalogoContexto --> ConsultandoPlantilla
  ConsultandoPlantilla --> SeleccionandoVersion
  SeleccionandoVersion --> ResolviendoTokens
  ResolviendoTokens --> RenderizandoHtml
  RenderizandoHtml --> Exitoso
  Rechazado --> [*]
  Exitoso --> [*]
```

## Diagrama de Casos de Uso

```mermaid
flowchart LR
  FE[Frontend] --> UC1((Obtener HTML inicial))
  UC1 --> UC2((Consultar estructura negocio))
  UC1 --> UC3((Validar ContextCode en catálogo))
  UC1 --> UC4((Consultar plantilla/version vigente))
  UC1 --> UC5((Resolver tokens))
  UC1 --> UC6((Renderizar HTML))
```

## Diagrama de Secuencia

```mermaid
sequenceDiagram
  autonumber
  participant FE as Frontend
  participant C as InitialContentEditorController
  participant S as ServiceInitialContentEditor
  participant SB as IServiceSolicitaEstructuraRespuesta
  participant CC as ISolicitaEditorContextDefinitionRepository
  participant TR as ITemplateDefinitionsRepository
  participant DB as BD

  FE->>C: GET /initial-content?idTareaWf&contextCode&entityId
  C->>C: Validar claim + parámetros
  C->>S: GetInitialContentAsync(...)
  S->>CC: Validar ContextCode activo
  CC->>DB: SELECT ra_editor_context_definitions
  S->>TR: GetDefinitionByCodeAsync(templateCode)
  TR->>DB: SELECT ra_editor_template_definitions
  S->>TR: GetVersionsByCodeAsync(templateCode)
  TR->>DB: SELECT ra_editor_template_versions
  S->>SB: SolicitaEstructuraRespuestaIdTareaAsync(idTareaWf)
  SB->>DB: SELECT ra_respuesta_radicado (por engine)
  S->>S: Resolver tokens + reemplazar en HTML
  S-->>C: htmlInicial + metadata
  C-->>FE: 200/400
```

## Secuencia literal (paso a paso)

1. Controller valida claim `defaulalias` y parámetros (`idTareaWf`, `contextCode`, `entityId`).
2. Service normaliza `ContextCode` (`Trim().ToUpperInvariant()`).
3. Service valida `ContextCode` activo en `ra_editor_context_definitions`.
4. Service selecciona `TemplateCode` (MVP: `TemplateCode == ContextCode`).
5. Service consulta definición y versiones de la plantilla.
6. Service elige la versión vigente (activa, prioriza publicada y mayor versión).
7. Service consulta estructura de negocio por `idTareaWf`.
8. Service construye mapa de tokens desde la estructura y reemplaza `{{TOKEN}}` en el HTML.
9. Retorna `htmlInicial` sin persistir documento.

## Diagrama de Clases

```mermaid
classDiagram
  class InitialContentEditorController
  class ServiceInitialContentEditor
  class IServiceSolicitaEstructuraRespuesta
  class ISolicitaEditorContextDefinitionRepository
  class ITemplateDefinitionsRepository

  InitialContentEditorController --> ServiceInitialContentEditor
  ServiceInitialContentEditor --> IServiceSolicitaEstructuraRespuesta
  ServiceInitialContentEditor --> ISolicitaEditorContextDefinitionRepository
  ServiceInitialContentEditor --> ITemplateDefinitionsRepository
```

