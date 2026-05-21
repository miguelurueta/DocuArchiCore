# SCRUM-208 Seguridad

## Controles
- Claim obligatorio: `defaulalias`.
- `idArchivo > 0`.
- `nombreGabinete` requerido y regex `^[A-Za-z0-9_]+$`.
- Alias solo desde claim.

## Datos
- Consulta mediante `DapperCrudEngine + QueryOptions`.
- Filtros parametrizados (`id_archivo`, `nombre_gabinete`).
- Sin SQL manual ni concatenación.

## Errores
- Respuesta controlada `AppResponses`.
- Sin exponer stacktrace interno al cliente.
