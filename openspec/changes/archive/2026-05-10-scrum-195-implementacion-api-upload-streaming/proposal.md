## Why

El endpoint actual de almacenamiento documental consume referencias temporales (`RutaTemporalId` + `ArchivoTemporalId`) y no está diseñado para recibir binario directo.  
Para integraciones externas y archivos de gran tamaño (hasta 30GB), se requiere una API de upload temporal con streaming/chunks que preserve la compatibilidad del Storage Engine actual.

## What Changes

- Implementar capacidad de upload temporal enterprise por chunks (`init`, `chunk`, `status`, `complete`, `cancel`).
- Soportar archivos de hasta `30GB` por agregación de chunks, evitando request monolítica.
- Persistir estado de sesión de upload y validar ownership por usuario.
- Integrar validación previa en `AlmacenarDocumento` para aceptar solo uploads `COMPLETED`.
- Definir operación en IIS con `StoragePaths__Temp`, límites, seguridad y limpieza TTL.
- Mantener sin cambios el contrato central de persistencia documental final (sin binario/URL externa).

## Capabilities

### New Capabilities
- `jira-scrum-195`: Upload temporal chunked para archivos grandes con integración al flujo de almacenamiento existente.

### Modified Capabilities
- `almacenamiento-documental`: agrega validación de referencia temporal completada antes de ejecutar el orquestador.

## Out of Scope

- Reemplazar el Storage Engine final.
- Recibir binario directamente en `POST /api/gestor-documental/almacenamiento`.
- Aceptar URL externa como fuente documental.
- Cambios de UI frontend fuera de pruebas técnicas de API.

## Impact

- Jira issue: https://contasoftcompany.atlassian.net/browse/SCRUM-195
- OpenSpec change path: `openspec/changes/scrum-195-implementacion-api-upload-streaming/`
- Reglas de implementación backend: `openspec/context/OPSXJ_BACKEND_RULES.md`
