## Why

`opsxj:archive` hoy archiva el cambio OpenSpec sin validar que el cambio ya haya sido integrado en Git, y tampoco actualiza el estado del ticket Jira. Esto permite cerrar cambios con trazabilidad incompleta.

## What Changes

- Se agrega validacion previa en `opsxj:archive` para comprobar en Git que la rama del cambio esta integrada en la rama base.
- Se agrega actualizacion de estado del issue en Jira a "Terminado/Done" al finalizar el archive exitosamente.
- Se agregan mensajes de error claros para validaciones Git y transicion de Jira.
- Se documenta la configuracion requerida para el nuevo flujo de archive.

## Capabilities

### New Capabilities
- `opsxj-archive-validation`: Validar integracion en Git y cerrar ticket Jira en `opsxj:archive`.

### Modified Capabilities
- Ninguna.

## Impact

- Cambios en `Tools/jira-open/opsxj.ps1` para validar merge en Git y transicionar Jira.
- Ajustes en `Tools/jira-open/README.md` y `.jira-open.env.example`.
- Nuevas pruebas de flujo para `opsxj:archive`.
- Repositorios afectados:
- `DocuArchiCore`
