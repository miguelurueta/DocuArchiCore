# Plantilla de Plan de Impacto de Repositorio

Use esta plantilla al inicio de cada ticket de Jira (`SCRUM-143`) para decidir exactamente dónde ejecutar `opsxj:new` y `opsxj:archive`.

## Ticket

- Clave Jira: `SCRUM-143`
- Resumen: `CREAR-API-ORCHES-ATOMICO-GUARDAR-DOCUMENTO-EDITOR`
- Cambio coordinador: `openspec/changes/scrum-143-crear-api-orches-atomico-guardar-documen/`

## Matriz de Impacto

| Repo | Impacta? | Tipo impacto | Motivo | `opsxj:new` | PR | `opsxj:archive` | Estado |
|---|---|---|---|---|---|---|---|
| DocuArchi.Api | yes | traceability_only | sin diff real publicado | n/a | n/a | pending | tracked |
| DocuArchiCore | yes | implementation_required | orquestador openspec central | done | https://github.com/miguelurueta/DocuArchiCore/pull/189 | pending | in_review |
| DocuArchiCore.Abstractions | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| DocuArchiCore.Web | no | no_code_change | solo consulta (sin cambios) | n/a | n/a | n/a | n_a |
| MiApp.DTOs | yes | traceability_only | sin diff real publicado | n/a | n/a | pending | tracked |
| MiApp.Services | yes | traceability_only | sin diff real publicado | n/a | n/a | pending | tracked |
| MiApp.Repository | yes | traceability_only | sin diff real publicado | n/a | n/a | pending | tracked |
| MiApp.Models | no | no_code_change | se reutilizan modelos existentes | n/a | n/a | n/a | n_a |

## Regla Operativa

Ejecute `opsxj:new SCRUM-143` solo en filas con `Impacta? = yes`.
Solo los repositorios marcados como `implementation_required` deben abrir rama/commit/PR.
Los repositorios marcados como `traceability_only` permanecen en `sync.md` sin abrir PRs vacíos.
Las filas con `Impacta? = no` deben permanecer como `n/a` para explicitar el alcance.

## Patrón Típico de Cambio de API

- `DocuArchi.Api`: generalmente sí
- `MiApp.Services`: generalmente sí
- `MiApp.Repository`: generalmente sí
- `MiApp.DTOs`: generalmente sí
- `MiApp.Models`: opcional (solo si cambian modelos)
- `DocuArchiCore` / `DocuArchiCore.Web`: solo si el coordinador o la UI están en alcance