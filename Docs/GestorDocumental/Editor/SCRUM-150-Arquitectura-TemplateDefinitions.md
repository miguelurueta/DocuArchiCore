# SCRUM-150 — Arquitectura: Catálogo de Plantillas HTML del Editor

## Propósito

Proveer un catálogo formal de plantillas reutilizables del editor Tiptap con **definición** + **versionado**, evitando hardcodeo en frontend/backend.

## Tablas

- `ra_editor_template_definitions`
- `ra_editor_template_versions`

## Diagrama de Estado

```mermaid
stateDiagram-v2
  [*] --> Validando
  Validando --> CreandoDef: request OK
  Validando --> Rechazado: validación/claim inválido
  CreandoDef --> Creada
  CreandoDef --> Rechazado: duplicado/error
  Creada --> [*]
  Rechazado --> [*]
```

## Diagrama de Casos de Uso

```mermaid
flowchart LR
  FE[Admin/Usuario técnico] --> UC1((Crear definición de plantilla))
  FE --> UC2((Crear versión de plantilla))
  FE --> UC3((Consultar plantilla por código))
```

## Diagrama de Secuencia

```mermaid
sequenceDiagram
  autonumber
  participant FE as Cliente
  participant C as TemplateDefinitionsController
  participant S as ServiceTemplateDefinitions
  participant R as TemplateDefinitionsRepository
  participant DB as BD

  FE->>C: POST /templates/definitions
  C->>C: Validar claim + request
  C->>S: CreateDefinitionAsync
  S->>R: CreateDefinitionAsync
  R->>DB: INSERT ra_editor_template_definitions
  DB-->>R: template_definition_id
  R-->>S: DefinitionDto
  S-->>C: AppResponses
  C-->>FE: 200/400
```

## Secuencia literal (paso a paso)

1. Controller valida claim `defaulalias`.
2. Controller valida `TemplateCode` y `TemplateName`.
3. Service normaliza `TemplateCode` (trim + upper).
4. Repository valida duplicado por `template_code`.
5. Inserta la definición y retorna la definición persistida.

## Diagrama de Clases

```mermaid
classDiagram
  class TemplateDefinitionsController
  class ServiceTemplateDefinitions
  class ITemplateDefinitionsRepository
  TemplateDefinitionsController --> ServiceTemplateDefinitions
  ServiceTemplateDefinitions --> ITemplateDefinitionsRepository
```

## Diagrama de Tablas

```mermaid
erDiagram
  ra_editor_template_definitions ||--o{ ra_editor_template_versions : template_definition_id
```

