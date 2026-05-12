## Why

El flujo `POST /api/gestor-documental/almacenamiento` falla con `GAB_FIELDS_NOT_FOUND`
porque la resolucion de metadata de gabinete sigue en placeholder y retorna lista vacia.
Adicionalmente no existe validacion backend completa de:

- existencia real del campo
- obligatoriedad backend desacoplada de estado UI
- compatibilidad de tipo
- longitud maxima permitida
- consistencia metadata vs estructura fisica de tabla

Esto bloquea almacenamiento documental para gabinetes como `CONTABIL`.

## What Changes

- Reemplazar provider placeholder por resolucion real de metadata desde DB (`detalle_gabienete`) por gabinete.
- Incorporar validacion estructural backend en pipeline antes de insertar:
  - campo desconocido
  - campo obligatorio vacio
  - tipo invalido
  - longitud invalida
- Agregar parser de tipos legacy (`VARCHAR(n)`, `CHAR(n)`, `INT`, `INTEGER`, `DATE`, `DATETIME`, `TEXT`, `LONGTEXT`).
- Incorporar cache de metadata por `alias + gabinete` con TTL configurable.
- Agregar validacion opcional de consistencia fisica contra `INFORMATION_SCHEMA.COLUMNS`.
- Estandarizar codigos de error funcionales para diagnostico y soporte.
- Definir pruebas unitarias/integracion/contrato y actualizar documentacion tecnica.

## Capabilities

### Modified Capabilities

- `jira-scrum-196`: validacion backend estructural por metadata real de gabinete.

## Impact

- Jira issue: https://contasoftcompany.atlassian.net/browse/SCRUM-196
- OpenSpec change path: `openspec/changes/scrum-196-implementacion-validacion-metada-data-ga/`
- Reglas tecnicas: `openspec/context/OPSXJ_BACKEND_RULES.md`
- Evidencia de estructura legacy: `C:\cert\resultado-detalle.txt`

Repos esperados con implementacion:

- `DocuArchi.Api`
- `MiApp.Services`
- `MiApp.Repository`
- `MiApp.Models`
- `MiApp.DTOs`
- `DocuArchiCore` (artefactos OpenSpec y documentacion)
