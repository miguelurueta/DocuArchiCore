# SCRUM-150 — Implementación Detallada: TemplateDefinitions

## Archivos

- API
  - `DocuArchi.Api/Controllers/GestorDocumental/Editor/TemplateDefinitionsController.cs`
  - `DocuArchi.Api/Program.cs` (DI)
- Services
  - `MiApp.Services/Service/GestorDocumental/Editor/ServiceTemplateDefinitions.cs`
- Repository
  - `MiApp.Repository/Repositorio/GestorDocumental/Editor/TemplateDefinitionsRepository.cs`
- DTOs
  - `MiApp.DTOs/DTOs/GestorDocumental/Editor/TemplateDefinitionsDtos.cs`
- Models
  - `MiApp.Models/Models/GestorDocumental/Editor/RaEditorTemplateDefinition.cs`
  - `MiApp.Models/Models/GestorDocumental/Editor/RaEditorTemplateVersion.cs`

## Endpoints

- `POST /api/gestor-documental/editor/templates/definitions`
- `POST /api/gestor-documental/editor/templates/versions`
- `GET /api/gestor-documental/editor/templates/definitions/{templateCode}`

## Reglas

- `TemplateCode` se normaliza (trim + upper) para evitar inconsistencias.
- Duplicado por `TemplateCode` → Validation.
- Duplicado por `VersionNumber` dentro de una plantilla → Validation.
- Se retorna “Sin resultados” cuando no existe definición al consultar por código.

## Script SQL recomendado (referencia)

```sql
CREATE TABLE IF NOT EXISTS ra_editor_template_definitions (
  template_definition_id BIGINT PRIMARY KEY AUTO_INCREMENT,
  template_code VARCHAR(100) NOT NULL,
  template_name VARCHAR(150) NOT NULL,
  module_name VARCHAR(100) NULL,
  description VARCHAR(255) NULL,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_by VARCHAR(100) NULL,
  created_at DATETIME NULL,
  UNIQUE KEY uq_ra_editor_template_definitions_code (template_code)
);

CREATE TABLE IF NOT EXISTS ra_editor_template_versions (
  template_version_id BIGINT PRIMARY KEY AUTO_INCREMENT,
  template_definition_id BIGINT NOT NULL,
  version_number INT NOT NULL,
  template_html LONGTEXT NOT NULL,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  is_published TINYINT(1) NOT NULL DEFAULT 0,
  created_by VARCHAR(100) NULL,
  created_at DATETIME NULL,
  UNIQUE KEY uq_ra_editor_template_versions_def_ver (template_definition_id, version_number),
  KEY ix_ra_editor_template_versions_def (template_definition_id)
);
```

