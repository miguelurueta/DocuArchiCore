## Why

Los cambios futuros impactan varios repositorios y hoy no existe una fuente unica y verificable de contexto tecnico para preparar propuestas y disenos cross-repo.

## What Changes

- Crear y mantener el documento consolidado `openspec/context/multi-repo-context.md`.
- Exigir referencia explicita al documento consolidado en cambios OpenSpec que impacten dos o mas repositorios.
- Estandarizar los datos minimos de contexto: inventario, estado Git y artefactos tecnicos.

## Capabilities

### New Capabilities
- `multi-repo-context`: Gestion de contexto consolidado para cambios OpenSpec cross-repo.

### Modified Capabilities
- None.

## Impact

- OpenSpec artifacts: `proposal.md`, `design.md`, `tasks.md`, `specs/multi-repo-context/spec.md`.
- Documentation process: cambios cross-repo ahora dependen de contexto consolidado actualizado.
- Affected repositories:
  - `DocuArchiCore`
  - `DocuArchi.Api`
  - `DocuArchiCore.Abstractions`
  - `MiApp.DTOs`
  - `MiApp.Models`
  - `MiApp.Repository`
  - `MiApp.Services`
- No runtime impact in application code.
