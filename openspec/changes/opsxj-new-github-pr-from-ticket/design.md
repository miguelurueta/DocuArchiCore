## Context

`opsxj:new` ya consulta Jira y genera artefactos OpenSpec para iniciar cambios. El nuevo requerimiento pide cerrar el flujo en el mismo comando: cuando `proposal.md` quede creado, el sistema debe abrir un PR en GitHub con el asunto del ticket.

## Goals / Non-Goals

**Goals:**
- Crear PR automaticamente durante `opsxj:new` despues de generar la propuesta.
- Usar el `summary` del ticket Jira como titulo del PR.
- Evitar duplicados y reportar errores accionables si GitHub falla.

**Non-Goals:**
- No cambiar el flujo de archivado (`opsxj:archive`).
- No implementar UI; el alcance sigue siendo CLI/script.
- No reemplazar Jira como fuente de verdad del ticket.

## Decisions

- **Usar GitHub CLI (`gh`) desde `opsxj.ps1`** para autenticacion y creacion de PR.
  - Alternativa: API REST directa con tokens y llamadas HTTP.
  - Razon: `gh` ya esta instalado en entorno de trabajo y reduce complejidad de autenticacion.
- **Crear PR solo despues de persistir `proposal.md`** para mantener consistencia del flujo.
  - Alternativa: crear PR antes de escribir artefactos.
  - Razon: evita PR sin contenido de cambio.
- **Titulo del PR basado en `summary` del issue Jira**.
  - Alternativa: usar `issueKey` como titulo principal.
  - Razon: el usuario pidio que el asunto del ticket gobierne el titulo del PR.
- **Estrategia idempotente por rama head + issueKey**.
  - Alternativa: crear siempre un PR nuevo.
  - Razon: evita duplicados cuando se re-ejecuta `opsxj:new`.

## Risks / Trade-offs

- [`gh` no autenticado o no disponible] -> Mitigacion: validar prerequisitos y abortar con mensaje explicito.
- [Fallo al crear PR tras generar proposal] -> Mitigacion: conservar archivos creados y reportar siguiente accion sugerida.
- [Conflicto de rama o permisos de push] -> Mitigacion: detectar error de push/PR y devolver diagnostico util.

## Migration Plan

- No requiere migracion de datos.
- Despliegue por script: actualizar `Tools/jira-open/opsxj.ps1`, luego validar con pruebas de flujo.

## Open Questions

- Definir rama base por defecto (`main`/`master`) o leerla dinamicamente de `origin/HEAD`.
- Definir formato final del cuerpo del PR (enlace Jira, resumen del cambio, checklist).
