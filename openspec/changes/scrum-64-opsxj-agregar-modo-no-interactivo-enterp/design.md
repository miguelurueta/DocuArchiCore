## Context

- Jira issue key: SCRUM-64
- Jira summary: OPSXJ: agregar modo no interactivo enterprise para opsxj:new manteniendo compatibilidad legacy
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-64

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

Descripción Se requiere incorporar un modo no interactivo explícito para opsxj:new, orientado a operación enterprise y compatibilidad con CI/CD, sin alterar el comportamiento actual por defecto. El nuevo modo debe activarse mediante un parámetro adicional, por ejemplo -NonInteractive, y debe ejecutar el flujo sin prompts ni dependencias de autenticación manual. El modo actual debe conservarse intacto para evitar regresiones operativas. Alcance: mantener opsxj:new <ISSUE-KEY> como flujo actual agregar opsxj:new <ISSUE-KEY> -NonInteractive extender el mismo criterio a opsxj:doctor y opsxj:archive exigir credenciales preconfiguradas para Jira y GitHub en modo no interactivo bloquear fallback interactivo de GitHub CLI en ese modo registrar auditoría del modo de ejecución documentar el modo enterprise/headless agregar pruebas automatizadas del nuevo comportamiento Criterios de aceptación opsxj:new <ISSUE-KEY> mantiene el comportamiento actual sin cambios funcionales. opsxj:new <ISSUE-KEY> -NonInteractive no solicita input interactivo. En -NonInteractive, el sistema falla si falta GITHUB_TOKEN. En -NonInteractive, el sistema no depende de gh auth login. opsxj:doctor -NonInteractive valida configuración requerida para ejecución headless. opsxj:archive -NonInteractive conserva ejecución no interactiva y trazabilidad. La auditoría incluye el modo de ejecución (legacy o noninteractive). Existen pruebas automatizadas del nuevo modo y siguen pasando las del modo actual. Se actualiza la documentación en Tools/jira-open/README.md. Notas Se recomienda registrar este trabajo con cambio OpenSpec asociado: opsxj-non-interactive-mode. El objetivo es habilitar operación segura sin intervención manual, manteniendo compatibilidad y rollback trivial.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-64-opsxj-agregar-modo-no-interactivo-enterp.