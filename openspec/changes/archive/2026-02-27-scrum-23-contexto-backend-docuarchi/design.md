## Context

El issue SCRUM-23 en este cambio se limita a ampliar el contexto OpenSpec para trabajo multi-repo. `DocuArchiCore` actua como coordinador de especificacion y trazabilidad.

## Context Reference

Este cambio usa como referencia `openspec/context/multi-repo-context.md` para inventario de repositorios y dependencias cruzadas.
Dependencias cross-repo clave:
- `DocuArchi.Api`, `MiApp.Services`, `MiApp.Repository`, `MiApp.DTOs`, `MiApp.Models` se mantienen como repos ejecutores.
- El coordinador solo documenta inventario, dependencias y estado en OpenSpec.

## Goals / Non-Goals

**Goals:**
- Definir reglas claras en `openspec/config.yaml` para cambios cross-repo.
- Mantener referencia obligatoria a `openspec/context/multi-repo-context.md`.
- Alinear trazabilidad de PR/estado en un tablero central (`sync.md`).

**Non-Goals:**
- No implementar cambios funcionales en API/Services/Repository/DTOs/Models como parte de este cambio.
- No modificar contratos, DI, pruebas o logica de negocio en repos ejecutores.
- No consolidar repositorios en un monorepo.

## Decisions

- `DocuArchiCore` centraliza contexto y seguimiento; no aplica cambios de codigo ejecutor.
- `openspec/config.yaml` define reglas obligatorias para propuestas y disenos cross-repo.
- `sync.md` centraliza estado por repo (PR/en progreso/bloqueos/evidencia).
- Las implementaciones de codigo se gestionan en cambios separados por repositorio ejecutor.

## Risks / Trade-offs

- [Confusion de alcance entre contexto y codigo] -> Mitigacion: declarar en proposal/spec/tasks que este cambio es context-only.
- [Desalineacion documental entre repos] -> Mitigacion: referencia obligatoria al contexto multi-repo y tablero `sync.md`.
