## Why

SCRUM-188 requiere consolidar trazabilidad y documentacion tecnica de paridad legacy (Inventario/TRD/Unidad) sin duplicar logica runtime ya implementada y mergeada en SCRUM-181.

## What Changes

- Refinar artefactos OpenSpec del ticket con alcance real de `traceability_only` y regresion documentada.
- Declarar explicitamente no-alcance de cambios funcionales en runtime para evitar duplicidad.
- Crear paquete documental enterprise en `Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine`.

## Capabilities

### New Capabilities
- jira-scrum-188: Consolidacion de trazabilidad y evidencia tecnica para opciones legacy `system1`.

### Modified Capabilities
- None.

## Impact

- Jira issue: https://contasoftcompany.atlassian.net/browse/SCRUM-188
- OpenSpec change path: openspec/changes/scrum-188-implementacion-inventario-trd-almacenami/
- Backend rules reference: openspec/context/OPSXJ_BACKEND_RULES.md
- Runtime delta en este repo: no aplica (sin cambios de comportamiento ejecutable).
