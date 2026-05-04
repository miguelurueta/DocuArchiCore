# SCRUM-178 — Observabilidad Preindex Integración

## Logs esperados
- `Warning`: preindex no encontrado en batch.
- `Information`: preindex encontrado con nombre de archivo y cantidad de valores.
- `Error`: fallo de lectura preindex.

## Códigos de error funcional
- `PREINDEX_NOT_FOUND`
- `PREINDEX_PATH_INVALID`
- `PREINDEX_INVALID_FORMAT`
- `PREINDEX_READ_ERROR`
- `PREINDEX_FIELDS_EMPTY`
- `PREINDEX_FIELDS_MISMATCH`

## Campos sensibles
No se registra contenido de documento ni valores completos de indexación fuera del flujo controlado.
Solo se expone:
- `requestId`
- nombre de archivo preindex
- cantidad de valores
