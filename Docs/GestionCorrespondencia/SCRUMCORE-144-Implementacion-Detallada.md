# SCRUMCORE-144 — Implementación Detallada

## Backend

### Contrato

- Se agrega `AppMeta.Status` (aditivo) para expresar `success|empty|pending`.
- `pending` solo es válido si existe señal objetiva (si no existe, no se usa).

### Repository (SQL / mapping)

- Se agrega selección explícita de `TRAMITE_DOCUMENTO` como `TramiteDocumento` si existe en tabla.
- Se retorna `meta.status`:
  - `empty` cuando no hay filas
  - `success` cuando hay filas

### Service

- Propaga `meta`.
- Normaliza `meta.status` cuando falte (compatibilidad si algún originador no lo setea).

### Controller

- Logs por request con `idTareaWf`, `defaulalias` y correlación (`TraceIdentifier` + `X-Request-Id` si viene).

## Frontend

- Consumir `meta.status` como fuente canónica cuando exista.
- Fallback legacy cuando no exista `meta.status`.
- Evitar bloqueo definitivo en `pending`.

## Compatibilidad

- No se cambia el shape de `AppResponses<T>`.
- `message` se mantiene como texto humano (no fuente de decisión).
