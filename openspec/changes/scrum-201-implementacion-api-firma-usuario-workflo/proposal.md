## Why

Se requiere exponer una API backend para entregar al frontend una firma de usuario workflow como recurso temporal consumible (URL/relative path), sin exponer el blob directo ni depender de IDs enviados por cliente.

## What Changes

- Crear endpoint de lectura de firma temporal workflow siguiendo patron `Controller -> Service -> Repository`.
- Usar claims obligatorios `defaulaliaswf` e `IdUsuarioWorkflow`.
- Consultar `usuario_workflow.Firma_Usuario` filtrando por `idU_suario` resuelto desde claim.
- Persistir temporal en carpeta dedicada y retornar metadata segura (`fileName`, `contentType`, `relativePath/urlTemporal`, `expiresAt`).
- Mantener contrato de salida basado en `AppResponses<T>`.
- Incluir pruebas unitarias/integracion y documentacion tecnica.

## Capabilities

### New Capabilities
- jira-scrum-201: API de firma temporal de usuario workflow para consumo frontend.

### Modified Capabilities
- Repositorio workflow de usuario para exponer lectura de blob de firma en metodo dedicado reutilizable.

## Impact

- Jira issue: https://contasoftcompany.atlassian.net/browse/SCRUM-201
- OpenSpec change path: openspec/changes/scrum-201-implementacion-api-firma-usuario-workflo/
- Backend rules reference: openspec/context/OPSXJ_BACKEND_RULES.md
- Repos impactados:
  - `DocuArchi.Api`
  - `MiApp.Services`
  - `MiApp.Repository`
  - `MiApp.DTOs`
  - `tests`
