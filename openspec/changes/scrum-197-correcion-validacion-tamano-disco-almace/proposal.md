## Why

El flujo de almacenamiento falla en fase transaccional con:
`Unknown column 'ESTADO_DISCO' in 'field list'`.

La causa es un desfase de migracion: el codigo C# depende de la columna
`disco_detalle.ESTADO_DISCO`, pero en ambientes productivos esa columna no existe.

La logica legacy VB (`Numero_Imagenes`) no dependia de esa columna; calculaba bloqueo de
capacidad con `TAMDISC + NUMERO_IMAGENES`.

## What Changes

- Corregir validacion de cuota de disco para que la fuente primaria sea la regla legacy
  (`tamdisc` + `numero_imagenes`) y no `ESTADO_DISCO`.
- Mantener compatibilidad opcional con `ESTADO_DISCO` cuando exista en otros ambientes.
- Eliminar dependencia obligatoria de esa columna en lectura transaccional de `disco_detalle`.
- Preservar orden transaccional y lock pessimista (`FOR UPDATE`) antes de commit.
- Mantener contratos HTTP sin cambios (correccion interna de engine/repository/policy).

## Capabilities

### Modified Capabilities
- `jira-scrum-197`: validacion de capacidad de disco con paridad legacy sin dependencia
  obligatoria de `ESTADO_DISCO`.

### Unchanged Capabilities
- Contrato API de almacenamiento.
- Flujo de upload temporal.

## Impact

- Jira issue: https://contasoftcompany.atlassian.net/browse/SCRUM-197
- OpenSpec change path: openspec/changes/scrum-197-correcion-validacion-tamano-disco-almace/
- Backend rules reference: openspec/context/OPSXJ_BACKEND_RULES.md
- Repos esperados con implementacion:
  - MiApp.Services
  - MiApp.Repository
  - MiApp.Models
  - DocuArchiCore (specs/docs/tests)
