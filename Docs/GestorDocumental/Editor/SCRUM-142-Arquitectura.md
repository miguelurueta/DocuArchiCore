# SCRUM-142 — Arquitectura

## Objetivo

Sincronizar las relaciones `documento ↔ imágenes` usadas por el editor (Tiptap) en la tabla `ra_editor_document_image_links`, a partir del estado actual del contenido (lista de `image_uid`).

## Flujo

Controller → Service → Repository → DB

1. Controller valida claim `defaulalias` + payload.
2. Service valida y orquesta (AppResponses + try/catch).
3. Repository resuelve `ImageUid → ImageId` (solo imágenes no eliminadas) y aplica sync transaccional:
   - elimina relaciones obsoletas
   - inserta relaciones faltantes

## Reglas

- Idempotente.
- Lista vacía limpia relaciones.
- No elimina imágenes, solo links.

