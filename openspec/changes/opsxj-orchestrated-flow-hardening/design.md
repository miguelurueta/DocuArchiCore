## Context

`DocuArchiCore` ya opera como orquestador OpenSpec/Jira/GitHub para cambios multi-repo. La ejecucion real de `SCRUM-90` confirmo que el modelo funciona, pero tambien expuso huecos operativos:

- repos satelite con checkout ocupado por otro ticket
- PRs satelite creados solo para trazabilidad
- archive bloqueado por ramas remotas borradas despues del merge
- residuos locales (`.tmp`, logs, worktrees`) que frenan el cierre

El objetivo no es redisenar `opsxj`, sino endurecer el flujo ya existente para que la operacion sea efectivamente de un solo comando.

## Goals / Non-Goals

**Goals:**
- Crear worktrees limpios automaticamente para repos impactados cuando el checkout principal no sea utilizable.
- Registrar en `sync.md` el tipo de impacto real por repo y usarlo para decidir si un PR es obligatorio.
- Permitir que `orchestrate:archive` cierre correctamente aunque la branch remota del cambio ya no exista, siempre que el PR este `MERGED`.
- Limpiar residuos temporales del flujo orquestado sin bloquear el archive.

**Non-Goals:**
- No mover OpenSpec fuera de `DocuArchiCore`; se mantiene como repositorio orquestador.
- No automatizar implementacion de codigo de negocio en repos satelite.
- No cambiar el flujo repo-local legacy (`opsxj:new`, `opsxj:archive`) salvo donde comparta helpers con el flujo orquestado.

## Decisions

- **Worktree automatico por repo impactado.**
  - Si el checkout principal esta sucio, en otra rama o con branch collision, el orquestador crea un worktree temporal y opera ahi.
  - Alternativa: fallar y pedir al usuario limpiar el repo.
  - Razon: elimina la principal fuente de intervencion manual en tickets multi-repo reales.

- **Clasificacion de impacto en lugar de `yes/no` simple.**
  - `implementation_required`: requiere branch/commit/PR real.
  - `traceability_only`: se registra en `sync.md`, pero no exige cambio de codigo ni PR si no hay diff.
  - `no_code_change`: repo referenciado solo para contexto.
  - Alternativa: seguir usando `impact=yes/no`.
  - Razon: evita PRs vacios y mejora la lectura operativa del cambio.

- **PR merged como fuente de verdad para archive satelite.**
  - Si el PR de un repo satelite esta `MERGED`, el archive no debe depender de que la rama remota siga existiendo ni de topologias git identicas.
  - Alternativa: exigir siempre `merge-base --is-ancestor`.
  - Razon: GitHub puede borrar la branch al mergear y esa condicion no invalida la trazabilidad del ticket.

- **Limpieza automatica limitada a residuos de `opsxj`.**
  - Se limpia solo lo que el flujo orquestado creo: worktrees temporales conocidos, logs del issue actual y temporales de `.tmp` asociados al cambio.
  - Alternativa: limpiar cualquier residuo del repo.
  - Razon: evita acciones destructivas sobre trabajo del usuario.

## Data / State Changes

- Extender `sync.md` para incluir un campo de tipo de impacto y un estado mas expresivo para PRs satelite.
- Mantener un registro temporal por issue bajo `.opsxj/orchestrator/` con metadata de worktrees creados y residuos a limpiar.
- Reutilizar el PR real de GitHub como validacion primaria en archive.

## Risks / Trade-offs

- [Crear worktrees automaticamente puede dejar residuos si una ejecucion falla a mitad] -> Mitigacion: registrar ownership del worktree y limpiarlo en reruns/archive.
- [Clasificacion de impacto incorrecta puede saltarse un PR requerido] -> Mitigacion: validacion explicita y mensajes claros en `sync.md`.
- [Basarse en PR merged puede ocultar un merge anomalo] -> Mitigacion: mantener chequeo git como senal secundaria cuando la branch siga disponible.
- [Limpieza automatica puede borrar temporales que el usuario queria inspeccionar] -> Mitigacion: limitarse a temporales del issue actual y documentar el comportamiento.
