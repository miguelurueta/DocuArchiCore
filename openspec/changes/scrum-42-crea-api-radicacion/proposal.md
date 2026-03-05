## Why

Se requiere implementar la API de radicacion entrante para migrar el flujo legacy `Registra_Radicacion_entrante` a arquitectura por capas en el ecosistema DocuArchiCore. El alcance exige paridad funcional con el legacy y reglas no negociables de seguridad SQL, trazabilidad, y atomicidad transaccional.

## What Changes

- Definir y aplicar contratos API para:
  - `POST /api/radicacion/registrar-entrante`
  - `POST /api/radicacion/validar-entrante`
  - `GET /api/radicacion/flujo-inicial`
- Implementar orquestacion por capas (`Controller -> Service -> Repository`) para reemplazar flujo legacy acoplado a WebForms.
- Migrar persistencia de `Registra_Radicacion_entrante` con matriz Q01-Q09 y politica de transaccion unica para Q01-Q08.
- Establecer compatibilidad de `Campos[]` y mapeo a DTO canonico en capa Service.
- Establecer pruebas unitarias/integracion/contract para validar paridad funcional y reglas de atomicidad.

## Capabilities

### New Capabilities
- jira-scrum-42-radicacion-api: API de radicacion entrante con validacion, flujo inicial y registro transaccional.

### Modified Capabilities
- radicacion-tramite: incorpora contratos y reglas de migracion para `Registra_Radicacion_entrante`.

## Impact

- Jira issue: https://contasoftcompany.atlassian.net/browse/SCRUM-42
- OpenSpec change path: openspec/changes/scrum-42-crea-api-radicacion/
- Backend rules reference: openspec/context/OPSXJ_BACKEND_RULES.md
- Documento maestro de alcance funcional:
  - Docs/MigracionCodigo/TiketJira/radicador/Registra_Radicacion_entrante/TICKETS-F1-F7-Registra_Radicacion_entrante.md
