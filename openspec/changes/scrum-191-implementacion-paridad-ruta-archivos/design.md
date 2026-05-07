## Context

- Jira issue key: SCRUM-191
- Jira summary: IMPLEMENTACION-PARIDAD-RUTA-ARCHIVOS
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-191
- Fuente de verdad legacy: `D:\imagenesda\GestorDocumental\promp\CORE-API\Almacenamiento\funcion-almacena-consolidad.txt`

## Problem Statement

La fase física/XML del StorageEngine aún puede desviarse del comportamiento legacy:
- resolución de rutas finales,
- convenciones de naming de principal/adjuntos/XML,
- estructura y actualización de XML de gabinete/índice,
- compensación ante fallos post-commit.

Estas desviaciones pueden romper compatibilidad con visor y trazabilidad documental histórica.

## Scope

### In Scope

- `StoragePathResolver` y `StoragePhysicalPathService` para ruta final legacy real.
- `StorageNamingService` para naming determinístico `DIG/FXL` y adjuntos `.000N`.
- `StorageFileWriter` para copia segura sin sobrescritura.
- `StorageXmlWriter` para XML documental legacy-compatible.
- `ExpedienteIndiceXmlService/Writer` para actualización de índice de expediente.
- `StorageCompensationManager` para rollback físico ante fallo en fase física/XML.
- pruebas unitarias/integración de fase física/XML.
- documentación técnica `SCRUM-191` en ruta `Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/`.

### Out of Scope

- cambios de reglas de negocio de inventario/TRD/opciones system1.
- cambios del contrato API público.
- cambios de modelo de datos fuera de la capa física/XML.

## Architectural Decisions

1. Orden de ejecución obligatorio:
- DB commit
- FileSystem
- XML documental
- XML índice expediente
- compensación si falla fase física.

2. Responsabilidades separadas:
- naming en `StorageNamingService`
- rutas en `StoragePathResolver`/`StoragePhysicalPathService`
- IO en `StorageFileWriter`
- XML en `StorageXmlWriter` y `ExpedienteIndiceXmlService`
- compensación en `StorageCompensationManager`.

3. Seguridad de ruta:
- prohibir traversal y rutas fuera de root permitido.
- no usar `%TEMP%` como destino final.

4. Política de colisión:
- archivo existente => error controlado, nunca sobrescribir.

5. Regresión legacy:
- cualquier desviación debe quedar explícita como "Cumple con desviación aprobada".

## Risk Analysis

- Riesgo: ruta final no compatible con legado.
- Mitigación: pruebas de ruta contra `SYSTEM1RUT` + gabinete/disco/carpeta.

- Riesgo: XML no compatible con consumidor legacy.
- Mitigación: validación de estructura y atributos contra ejemplos legacy.

- Riesgo: archivos huérfanos por fallos post-commit.
- Mitigación: compensación y pruebas de rollback físico.

## No-Regression Constraints

- No introducir cambios en comandos `opsxj`.
- No modificar pipeline transaccional más allá de integración con fase física.
- Mantener inyección DI existente y contratos de interfaces.

## Validation Strategy

- Unit tests: naming, ruta, XML, colisión, compensación.
- Integration tests: copia real + generación XML + update XML índice + compensación.
- Legacy regression checklist: ruta/naming/xml adjuntos/índice.
