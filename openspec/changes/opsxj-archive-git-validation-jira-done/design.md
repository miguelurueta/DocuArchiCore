## Context

El comando `opsxj:archive` se usa para cerrar cambios OpenSpec asociados a tickets Jira. Para que el cierre sea consistente con el flujo operativo, el comando debe validar que el cambio ya fue aplicado en Git y despues mover el issue Jira a estado terminado.

## Goals / Non-Goals

**Goals:**
- Verificar antes de archivar que la rama del cambio ya esta integrada en la rama base de Git.
- Al archivar exitosamente, transicionar el ticket Jira a estado Done/Terminado.
- Mantener mensajes de error accionables y trazables.

**Non-Goals:**
- No cambiar el flujo de creacion (`opsxj:new`).
- No redisenar workflows Jira completos ni configuracion de board.
- No forzar un estado Jira unico; se selecciona por nombres comunes de cierre.

## Decisions

- **Validacion Git basada en ancestro** con `git merge-base --is-ancestor`.
  - Alternativa: validar por PR en GitHub.
  - Razon: requisito pide validacion en repositorio Git y evita dependencia directa de GitHub.
- **Rama de cambio por nombre `changeName`** y rama base por `GIT_BASE_BRANCH` o deteccion automatica.
  - Alternativa: suponer siempre `main`.
  - Razon: flexibilidad con repositorios que usan otras ramas base.
- **Transicion Jira via API de transitions** buscando estados de cierre comunes (`Done`, `Terminado`, `Closed`, etc.).
  - Alternativa: usar un ID fijo de transition.
  - Razon: los IDs cambian entre proyectos/workflows.

## Risks / Trade-offs

- [No existe rama local del cambio] -> Mitigacion: error explicito indicando que debe integrarse primero.
- [Workflow Jira sin estado de cierre esperado] -> Mitigacion: listar estados disponibles en el error.
- [Credenciales Jira ausentes] -> Mitigacion: error claro con variables requeridas.
