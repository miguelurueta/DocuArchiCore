## Context

- Jira issue key: SCRUM-188
- Jira summary: IMPLEMENTACION-INVENTARIO-TRD-ALMACENAMIENTO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-188

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

SCRUM-188 consolida la trazabilidad de paridad legacy para opciones `system1` (Inventario/TRD/Unidad) y documenta evidencia de regresion.

La implementacion funcional de estas reglas ya fue consolidada en el ticket previo SCRUM-181, por lo que este cambio conserva enfoque de:

- trazabilidad OpenSpec/Jira,
- validacion de alcance multi-repo,
- documentacion tecnica de soporte para auditoria y merge seguro.

No se introducen nuevos componentes runtime en este repositorio para evitar duplicidad funcional.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-188-implementacion-inventario-trd-almacenami.

## Scope Decision

- `DocuArchiCore`: implementacion requerida solo para artefactos OpenSpec + documentacion enterprise del ticket.
- `MiApp.Services`, `MiApp.Repository`, `MiApp.Models`, `MiApp.DTOs`: `traceability_only` para SCRUM-188 (sin delta funcional adicional respecto a SCRUM-181).

## Deliverables in This Repo

- OpenSpec actualizado (`proposal`, `design`, `tasks`, `spec`).
- Paquete documental en `Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine`:
  - Arquitectura,
  - Implementacion detallada,
  - Pruebas,
  - Observabilidad,
  - Regresion legacy,
  - Metadata del ticket.
