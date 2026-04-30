## Context

- Jira issue key: SCRUM-174
- Jira summary: ACTUALIZA-OPSXJ:NEW ORCHESTRATE
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-174

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

Contexto repo: Script principal: Tools/jira-open/opsxj.ps1 Comandos afectados: orchestrate:publish, archive, orchestrate:archive Hoy archive fuerza "-y" y permite continuar con tasks incompletas. Cambios requeridos: Agregar validador central: Leer openspec/changes/<change>/tasks.md Contar tareas pendientes con patrón "- [ ]" Exponer detalle (líneas pendientes) para mensaje de error/log. En Invoke-OrchestratePublish: Antes de publicar PRs satélite, bloquear si hay tareas pendientes. Error claro: "Policy enforced: incomplete tasks in tasks.md. Complete tasks before opsxj:orchestrate:publish." En Invoke-Archive e Invoke-OrchestrateArchive: Bloquear si hay tareas pendientes. Quitar bypass implícito por "-y" para este caso. Mantener -y para no-interactivo solo si tasks están completas. Override explícito (opcional pero recomendado): Nuevo switch: -ForceIncompleteTasks Si se usa, permitir continuar pero: Log estructurado con step específico (status=warning) Mensaje visible en consola indicando override Sin ese switch, siempre bloquear. Logs/auditoría: Escribir en Write-OpsxjLog: step: "tasks_validation" status: ok | error | warning data: pendingCount, pendingItems, forceOverride Documentación: Actualizar Tools/jira-open/README.md Actualizar Docs/Guia_Opsxj_MultiRepo.md Incluir regla: publish/archive requieren tasks completas. Pruebas: Crear/ajustar tests en Tools/jira-open: publish falla con tasks incompletas archive falla con tasks incompletas publish/archive pasan con tasks completas override permite continuar y deja log warning Ejecutar tests relevantes y mostrar salida resumida. Paso obligatorio de revisión humana de artefactos OpenSpec: Antes de permitir publish/archive (sin override), mostrar checklist y exigir confirmación explícita: proposal.md revisado design.md revisado specs/*/spec.md revisado tasks.md actualizado y completo sync.md consistente con repos impactados Registrar en log: step: "openspec_review_gate" status: ok | error data: reviewedBy, reviewedAt, changeName Si no hay confirmación, bloquear con mensaje: "Policy enforced: OpenSpec artifacts must be reviewed before publish/archive." Criterios de aceptación: Sin override: no se puede publish/archive con tasks pendientes. Con override explícito: se permite y queda auditado. Sin revisión OpenSpec confirmada: no se puede publish/archive. No romper flujos actuales con tasks completas. Documentación actualizada y consistente. Entrega esperada: Diff de archivos modificados Resultado de tests Resumen de riesgos y compatibilidad

## Approach

- Convertir requerimientos del issue en deltas OpenSpec claros y testeables.
- Aplicar restricciones de repositorio, arquitectura y pruebas de OPSXJ_BACKEND_RULES.
- Definir alcance y no-alcance antes de implementar.
- Validar con openspec.cmd validate scrum-174-actualiza-opsxj-new-orchestrate.