# SCRUM-145 — Arquitectura — LimpiezaEditorImages

## Caso de uso

Eliminar lógicamente (`DeletedAt`) imágenes del editor Tiptap que quedaron **huérfanas** (sin relación activa en `ra_editor_document_image_links`).

## Flujo

`Controller -> Service -> Repository`

- Controller valida claim `defaulalias` y request.
- Service valida reglas y orquesta `DryRun` vs marcado real.
- Repository ejecuta SQL (MySQL) para contar y/o marcar huérfanas.

## Decisiones

- Fase actual: **solo** borrado lógico (`DeletedAt`), sin borrado físico.
- Filtro por antigüedad: `OlderThanMinutes` para reducir riesgo operacional.
- Límite opcional: `Limit` para ejecución controlada.
