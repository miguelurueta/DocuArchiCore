## Context

- Jira issue key: SCRUM-127
- Jira summary: Integrar politica operativa de agentes Codex en flujo opsxj backend
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-127

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

Título [OPSXJ_CODEX_AGENTS] Integrar politica operativa de agentes Codex en flujo opsxj backend Tipo Tarea Proyecto sugerido SCRUM o el proyecto backend que uses Resumen funcional Se requiere incorporar en el repositorio backend DocuArchiCore una política operativa para uso de agentes Codex dentro del flujo opsxj, con el  objetivo de reducir consumo del agente principal sin perder control sobre operaciones críticas de Jira, Git, PR y archive multi-repo. Descripción Actualmente el flujo opsxj del backend ya soporta comandos operativos avanzados como new, orchestrate:new, orchestrate:publish, archive, orchestrate:archive, jira-done, doctor y jira-pending en Tools/jira-open/opsxj.ps1. Se requiere formalizar una política de uso de agentes Codex para este flujo, dejando explícito qué tareas deben ejecutarse con subagentes mini  y cuáles deben mantenerse en el agente principal. La implementación no debe intentar forzar técnicamente el modelo desde PowerShell, porque esa decisión pertenece al runtime de Codex. En cambio, debe dejar: documentación operativa clara hints visibles en la salida del flujo consistencia entre README, comandos y política documental Objetivo Reducir consumo del agente principal en fases de exploración, OpenSpec, documentación y análisis focal, manteniendo el agente principal para operaciones de gobierno del flujo. Alcance Actualizar el backend para incluir política de agentes Codex aplicada a: opsxj new opsxj orchestrate:new opsxj orchestrate:publish opsxj archive opsxj orchestrate:archive opsxj jira-done opsxj doctor Cambios requeridos Crear documento técnico de política de agentes, por ejemplo: Docs/Codex-Agent-Strategy.md Actualizar: Tools/jira-open/README.md Extender: Tools/jira-open/opsxj.ps1 Agregar una función reusable para hints operativos en salida, por ejemplo: Write-CodexAgentHint Política requerida Usar subagente mini para: lectura y resumen de ticket Jira proposal/design/spec/tasks clasificación de impacto revisión puntual de repos afectados análisis de DTOs, repositories y services revisión de logs y pruebas focales documentación técnica Usar agente principal para: opsxj:new opsxj:orchestrate:new opsxj:orchestrate:publish opsxj:archive opsxj:orchestrate:archive opsxj:jira-done validación final operaciones Git/GitHub/Jira decisiones multi-repo y cierre de flujo Criterios de aceptación Existe un documento de política de agentes Codex en el repo backend. Tools/jira-open/README.md referencia explícitamente esa política. opsxj.ps1 imprime sugerencias operativas de agentes según el comando ejecutado. La documentación deja claro que el script no controla el modelo de Codex, solo orienta el uso. La política diferencia correctamente mini vs principal. No se rompe el flujo actual de opsxj. Resultado esperado El backend queda alineado con una política operativa de consumo más eficiente para Codex, sin modificar el comportamiento funcional de Jira/ GitHub/OpenSpec. Archivos objetivo Tools/jira-open/opsxj.ps1 Tools/jira-open/README.md Docs/Codex-Agent-Strategy.md Nota técnica La implementación debe ser documental y operativa. No debe intentar integrar selección automática de modelo dentro del script PowerShell.

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-127-integrar-politica-operativa-de-agentes-c.