# SCRUM-180 — Observabilidad Ruta Física Legacy

## Logs Incorporados/Relevantes
- `StoragePhysicalPathService`
  - `requestId`
  - `gabinete`
  - `disco`
  - `carpeta`
  - Evento: `ruta fisica legacy resuelta`
- `StorageFileWriter`
  - Archivo origen/destino por nombre (sin exponer path completo sensible).
- `StorageXmlWriter`
  - Nombre de XML generado.

## Datos Sensibles Protegidos
- No se loguea contenido documental.
- No se loguea fulltext.
- No se loguea ruta física completa en mensajes funcionales de negocio.

## Señales Operativas
- Error de ruta inexistente en DB: falla temprana controlada.
- Error de path traversal: falla con `StoragePhysicalException`.
- Error de IO: compensación activa en fase física.

## Indicadores Recomendados
- Conteo de fallas por código `StoragePhysicalException`.
- Tiempo de resolución de ruta física.
- Distribución de fallas por gabinete/disco/carpeta.
