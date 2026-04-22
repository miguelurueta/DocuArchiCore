# SCRUMCORE-144 — Observabilidad

## Objetivo

Permitir diagnosticar la inconsistencia (vacío vs datos) para el mismo `idTareaWf`.

## Logs mínimos por request (backend)

- `idTareaWf`
- `defaulalias`
- request id (`HttpContext.TraceIdentifier`)
- `X-Request-Id` si viene en headers
- base resuelta (`SELECT DATABASE()`)
- `rows.Count`
- duración (ms)
- `meta.status` resultante

## Lectura de señales

- Alternancia de `db` o `alias` entre requests correlacionados indica problema de resolución/fuente.
- `empty` seguido de `success` sin cambio real implica:
  - inconsistencia de origen (alias/réplica) o
  - inserción tardía (eventual consistency) que debe expresarse con señal objetiva si se soporta `pending`.

