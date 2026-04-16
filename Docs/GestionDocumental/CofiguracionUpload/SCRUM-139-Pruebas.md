# SCRUM-139 — Pruebas — Configuración Upload

## Unit tests

- Reusa el set de unit tests del repository (SCRUM-138) y agregar tests del controller/service si aplica.

## Integración / QT

- Pendiente de entorno Docker/Testcontainers si se requiere E2E.

## Matriz de cobertura (mínima)

- Claim inválido → `BadRequest`
- `nameProceso` inválido → `BadRequest`
- Service ok con data → `Ok`
- Service ok sin data → `Ok` con `data=[]`
- Service error → `BadRequest`

