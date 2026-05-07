## Why

El flujo de almacenamiento documental aún presenta brechas de paridad física/XML frente al legacy VB:
- rutas finales inconsistentes,
- naming de archivos no totalmente legacy-compatible,
- riesgo de incompatibilidad de XML documental y XML de índice.

Esto afecta interoperabilidad con visor y procesos documentales históricos.

## What Changes

- Implementar paridad física real desde `SYSTEM1RUT` + gabinete + disco + carpeta.
- Asegurar naming legacy `DIG########.ext`, `FXL########.xml` y adjuntos `.0000`, `.0001`.
- Alinear estructura XML documental y actualización XML índice expediente con semántica legacy.
- Fortalecer compensación física post-commit y hardening de rutas.
- Documentar comparación VB vs C# y evidencia de pruebas/regresión.

## Capabilities

### New Capabilities
- jira-scrum-191: Cierre de paridad física/XML legacy para almacenamiento documental.

### Modified Capabilities
- storage-engine-physical-phase
- storage-engine-xml-generation
- storage-engine-expediente-indice-xml

## Impact

- Jira issue: https://contasoftcompany.atlassian.net/browse/SCRUM-191
- OpenSpec change path: openspec/changes/scrum-191-implementacion-paridad-ruta-archivos/
- Backend rules reference: openspec/context/OPSXJ_BACKEND_RULES.md
