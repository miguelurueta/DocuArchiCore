## Why

El flujo actual de `opsxj:new` funciona para uso local, pero no expone un modo enterprise explicito para ejecucion no interactiva con credenciales preautorizadas, politica deterministica y compatibilidad con CI/CD.

Hoy ya existen capacidades headless parciales:
- configuracion por `.jira-open.env` o variables de entorno
- validaciones previas estrictas
- seleccion interactiva de repos deshabilitada por policy
- archivado no interactivo con `openspec archive -y`

Sin embargo, todavia hay caminos no aptos para automatizacion enterprise:
- fallback a autenticacion GitHub por `gh`
- mensajes que remiten a `gh auth login`
- falta de un switch explicito para separar modo legacy y modo no interactivo
- auditoria sin una marca formal del modo de ejecucion

## What Changes

- Agregar un switch `-NonInteractive` a `opsxj:new`, `opsxj:doctor` y `opsxj:archive`.
- Mantener el flujo actual como comportamiento por defecto.
- En modo `-NonInteractive`, exigir credenciales preconfiguradas y bloquear fallbacks interactivos.
- Extender la auditoria para registrar el modo de ejecucion y metadatos operativos relevantes.
- Actualizar documentacion y pruebas automatizadas del flujo `opsxj`.

## Capabilities

### Modified Capabilities
- `opsxj:new`: soportar modo no interactivo explicito sin romper el modo legacy.
- `opsxj:doctor`: validar prerequisitos del modo no interactivo.
- `opsxj:archive`: aceptar el mismo perfil de ejecucion y registrar trazabilidad consistente.

## Impact

- Jira issue: https://contasoftcompany.atlassian.net/browse/SCRUM-64
- OpenSpec change path: openspec/changes/scrum-64-opsxj-agregar-modo-no-interactivo-enterp/
- Script principal: Tools/jira-open/opsxj.ps1
- Documentacion: Tools/jira-open/README.md
- Pruebas: Tools/jira-open/test-opsxj-*.ps1
