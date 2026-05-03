# SCRUM-170 Seguridad Paths

## Reglas
- `nombreGabinete` y `rutaTemporalId`: solo `[a-zA-Z0-9_-]`.
- `archivoTemporalId`: sin `..`, sin separadores de ruta, sin caracteres inválidos de FS.
- rutas finales/temporales siempre normalizadas con `Path.GetFullPath`.
- validación de pertenencia al root esperado (`StartsWith` sobre root normalizado).

## Ataques bloqueados
- `../../../archivo`
- rutas absolutas inyectadas
- uso de separadores `/` y `\` en IDs de archivo temporal
