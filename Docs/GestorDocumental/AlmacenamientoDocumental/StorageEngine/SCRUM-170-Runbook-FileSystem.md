# SCRUM-170 Runbook FileSystem

## Diagnóstico rápido
1. Validar `requestId` en logs de orquestador y fase física.
2. Verificar evento de error (`error fase fisica`) y mensaje asociado.
3. Confirmar si ejecutó compensación (`compensation completada`).

## Casos comunes
- archivo temporal no existe: revisar origen en `storage-temp/<rutaTemporalId>/<archivoTemporalId>`.
- destino existente: validar idempotencia/reintentos.
- path inválido: revisar sanitización de `rutaTemporalId` y `archivoTemporalId`.

## Recuperación manual
- si hay residuos, limpiar archivos parciales en carpeta final de storage.
- no modificar registros DB en esta fase; escalar a flujo de remediación DB+FS si aplica.
