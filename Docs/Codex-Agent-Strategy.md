# Codex Agent Strategy para opsxj

## Objetivo

Reducir consumo del agente principal en el flujo `opsxj` sin perder control sobre operaciones criticas de Jira, Git, GitHub, PR y archive multi-repo.

## Regla base

- Esta politica es operativa y documental.
- `opsxj.ps1` no selecciona tecnicamente el modelo de Codex.
- La decision de modelo pertenece al runtime de Codex.
- El script solo imprime hints visibles para orientar el uso recomendado.

## Usar subagente mini

Aplicar mini cuando la tarea sea de exploracion, analisis focal o documentacion:

- lectura y resumen del ticket Jira
- lectura de `proposal`, `design`, `spec`, `tasks`
- clasificacion de impacto por repos
- revision puntual de DTOs, repositories y services
- revision de logs, errores y pruebas focales
- preparacion o actualizacion de documentacion tecnica

## Usar agente principal

Mantener el agente principal para operaciones de gobierno del flujo:

- `opsxj:new`
- `opsxj:orchestrate:new`
- `opsxj:orchestrate:publish`
- `opsxj:archive`
- `opsxj:orchestrate:archive`
- `opsxj:jira-done`
- validacion final
- operaciones Git/GitHub/Jira
- decisiones multi-repo y cierre del flujo

## Matriz operativa

| Comando | Mini recomendado | Principal obligatorio |
|---|---|---|
| `opsxj:new` | resumen Jira/OpenSpec | ejecucion del comando, branch y PR |
| `opsxj:orchestrate:new` | clasificacion de impacto y lectura por repo | coordinacion multi-repo y PR coordinador |
| `opsxj:orchestrate:publish` | revision de diffs y pruebas focales | publicacion de PRs y validacion de repos |
| `opsxj:archive` | revision documental previa | merge checks, Jira done y archive |
| `opsxj:orchestrate:archive` | revision de evidencias por repo | cierre multi-repo, cleanup y archive |
| `opsxj:jira-done` | no aplica | operacion principal |
| `opsxj:doctor` | analisis del reporte y logs | ejecucion y decisiones correctivas |

## Restricciones

- No intentar controlar el modelo desde PowerShell.
- No convertir esta politica en automatizacion forzada de runtime.
- No mover al subagente mini operaciones criticas de Jira, Git o GitHub.

## Referencias

- `Tools/jira-open/README.md`
- `Tools/jira-open/opsxj.ps1`
