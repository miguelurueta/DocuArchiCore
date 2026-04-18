# SCRUM-145 — Integración Frontend — LimpiezaEditorImages

## Endpoint

- **Método**: `POST`
- **Rutas**:
  - `api/gestor-documental/editor/images/cleanup`
  - `api/gestor-documental/editor/images/cleanup/dry-run`
- **Auth**: requiere claim `defaulalias`

## Request

`LimpiezaEditorImagesRequestDto`

- `DryRun` (bool): si `true` no actualiza datos; solo informa cuántas imágenes serían afectadas.
- `OlderThanMinutes` (int?): solo considera huérfanas con `CreatedAt` nulo o `<= now - OlderThanMinutes`. Si `null` o `0`, no filtra por antigüedad.
- `Limit` (int): máximo de registros a retornar/marcar. Si `0`, sin límite.

## Response

`AppResponses<LimpiezaEditorImagesResponseDto?>`

- `data.TotalOrphansDetected`: cuántas imágenes huérfanas cumplen los filtros.
- `data.TotalMarkedAsDeleted`: cuántas imágenes fueron marcadas con `DeletedAt` (0 en `DryRun=true`).
- `data.ImageIds`: ids de imágenes consideradas (acotado por `Limit`).

## Ejemplo (dry-run)

```json
{
  "DryRun": true,
  "OlderThanMinutes": 60,
  "Limit": 100
}
```

## Notas operativas

- En esta fase **NO** se hace borrado físico; solo se marca `DeletedAt`.
