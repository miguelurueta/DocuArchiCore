## Context

- Jira issue key: SCRUM-199
- Jira summary: CORRECION-DA-EXTENSION-ALMACENAMIENTO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-199
- Error actual: fallo físico post-commit por resolución incorrecta de extensión (`No se pudo determinar extensión para tipo 43`).

## Problem

El flujo actual resuelve extensión y clasificación técnica de archivo desde `TRD.IdTipoDocumento`.  
En legacy, esa clasificación técnica se resuelve desde `DA_EXTENSION` por extensión real del archivo (`ESTENSION`), y de ahí se derivan `ESTADO_NORMAL`, `ESTADO_ADJUNTO`, `ESTADO_LINK`.

## Scope

En alcance:
- Corregir lookup técnico de archivo para usar `DA_EXTENSION` por extensión.
- Corregir origen de `DBT` para insert en gabinete (`DBT = ESTADO_NORMAL`).
- Introducir validación temprana (antes de commit DB) cuando no exista mapeo en `DA_EXTENSION`.
- Ajustar pruebas y documentación de StorageEngine.

Fuera de alcance:
- Cambios de contrato HTTP.
- Refactor general del orquestador fuera de este problema.

## Architectural Decisions

1. Separación de dominios:
- `TRD` mantiene clasificación archivística.
- `DA_EXTENSION` es fuente de verdad para clasificación técnica de archivo.

2. Momento de validación:
- La resolución `DA_EXTENSION` se valida antes de fase física y antes de commit transaccional.
- No se permite llegar a error físico tardío por mapeo faltante.

3. Regla de gabinete:
- Campo `DBT` del gabinete se alimenta exclusivamente de `DA_EXTENSION.ESTADO_NORMAL`.
- `TIME1` debe poblarse siempre por ser `NOT NULL` en `contabil`.

## Technical Plan

1. Resolver extensión efectiva:
- Fuente primaria: metadato físico.
- Fallback: `request.Documentos[].Extension`.
- Normalizar extensión para consulta legacy (equivalente funcional a `UCase`).

2. Repositorio de extensión:
- Consultar `DA_EXTENSION` por `ESTENSION`.
- Retornar estructura completa con `ESTADO_NORMAL`, `ESTADO_ADJUNTO`, `ESTADO_LINK`, `TIPO_ARCHIVO`, `PROGRAMA`.

3. Consumo en flujo:
- Usar resultado de `DA_EXTENSION` para `DBT`.
- Mantener `TRD.IdTipoDocumento` como informativo archivístico, no como fuente técnica.

4. Validación temprana:
- Si no hay mapeo en `DA_EXTENSION`, devolver error funcional explícito antes de commit.

## Observability

En caso de error de mapeo, registrar:
- `requestId`
- extensión detectada
- alias DB
- gabinete
- `TRD.IdTipoDocumento` (solo diagnóstico)

Mensaje esperado:
- `No existe mapeo en DA_EXTENSION para extensión <EXT>`

## Validation

- `openspec validate scrum-199-correcion-da-extension-almacenamiento`
- pruebas unitarias/integración de flujo storage para casos con y sin mapeo `DA_EXTENSION`.
