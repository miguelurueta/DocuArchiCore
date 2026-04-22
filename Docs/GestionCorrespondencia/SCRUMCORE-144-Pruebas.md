# SCRUMCORE-144 — Pruebas

## Unitarias Backend (mínimas)

- Con datos → `meta.status="success"`
- Sin datos definitivos → `meta.status="empty"`
- `TramiteDocumento` se mapea cuando exista

## Integración Backend

- Llamadas repetidas al mismo `idTareaWf` no alternan `empty`↔`success` sin explicación (alias/base/cambio real de datos).

## Frontend

- `empty` mantiene bloqueo sin flicker.
- `success` renderiza detalle directamente.
- `pending` muestra skeleton y no bloqueo definitivo.
- Fallback: ausencia de `meta.status` no rompe.

## QT

- Abrir `/dashboard/gestion-correspondencia/respuesta/934` y validar ausencia de parpadeo.
