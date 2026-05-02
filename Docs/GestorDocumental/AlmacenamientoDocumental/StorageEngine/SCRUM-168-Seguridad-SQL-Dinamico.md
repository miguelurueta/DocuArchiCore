# SCRUM-168 Seguridad SQL Dinamico

## Threat model basico
- Riesgo principal: SQL injection por nombre de tabla o columna dinamica.
- Superficie de ataque:
  - `NombreGabinete` recibido por flujo de almacenamiento.
  - claves de `CamposDinamicos`.
  - posibles intentos de concatenacion SQL legacy.

## Controles requeridos
- Validar identificadores con patron estricto: `^[a-zA-Z0-9_]+$`.
- Rechazar espacios, puntos, guiones, comillas, backticks y simbolos especiales.
- Parametrizar todos los valores de negocio.
- Limitar SQL dinamico solo a identificadores validados.

## Ejemplos permitidos
- `gabinete_entrada_2026`
- `CAMPO_1`
- `IDEX`

## Ejemplos bloqueados
- `gabinete;DROP TABLE system1`
- `campo nombre`
- `campo-1`
- `` `campo` ``
- `campo.origen`

## Justificacion de parametrizacion
- Evita inyeccion en valores.
- Mejora trazabilidad de auditoria y consistencia entre engines.
- Mantiene separacion entre estructura SQL e inputs de negocio.

## DapperCrudEngine
- Regla del ticket: repositories deben resolver operaciones mediante `DapperCrudEngine + QueryOptions`.
- Estado actual:
  - `MiApp.Repository` implemento insercion de gabinete e inventario usando `IDapperCrudEngine`.
  - La construccion SQL dinamica se limita a identificadores validados.
  - Los valores de negocio viajan como parametros para prevenir injection por payload.

## Deuda tecnica
- Definir whitelist por metadata real de gabinete (`IStorageGabineteMetadataProvider`) para restringir columnas dinamicas permitidas.
- Confirmar governance de cambios de esquema por gabinete.
