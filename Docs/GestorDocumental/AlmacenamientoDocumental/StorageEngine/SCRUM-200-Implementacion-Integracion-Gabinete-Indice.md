# SCRUM-200 - Implementacion Integracion Gabinete Dinamico e Indice Electronico

## Objetivo
- Cerrar brecha de paridad transaccional: reservar identidad, actualizar cuota, insertar gabinete dinámico, inventario e índice lógico en el mismo flujo transaccional.

## Cambios implementados
- `StorageTransactionCoordinator` integra secuencia completa pre-commit:
  1. `system1` (identity reservation).
  2. `disco_detalle` (lock/update cuota).
  3. gabinete dinámico (`context.NombreGabinete`).
  4. `registro_producion_documental` (cuando aplica).
  5. expediente/unidad (cuando aplica).
  6. `ra_cert_indice_expediente` (cuando aplica).
  7. workflow log (cuando aplica).
- `DBT` se toma desde `DA_EXTENSION.ESTADO_NORMAL` por extensión real.
- Homologación de tipología para evitar `NA`:
  - primero descriptivo resuelto,
  - fallback a valor recuperado de gabinete por `IdAlmacen`,
  - fallback final a `Trd.IdTipoDocumento`.
- Inserción de índice lógico:
  - `Nombre_documento` = nombre físico final (`DIG########.EXT`),
  - `Tipologia_documental` = tipología homologada,
  - `ruta_documento` = ruta final de almacenamiento.
- Inventario:
  - `DESCRIPCION_TIPO_DOCUMENTO` homologada con `TIPODOCUMENTO`,
  - descriptivos TRD (`NOMBRE_AREA_DEPARTAMENTO`, `SERIE_DOCUMENTO`, `SUBSERIE_DOCUMENTO`) desde request o fallback metadata,
  - `UNIDADCONSERVA` queda `NULL` cuando no existe código FK-compatible.

## Compatibilidad de XML físico de expediente
- Se conserva escritura de XML de índice físico con estructura legacy compatible.
- Se corrige llenado de tipología para no persistir `NA`.

## Observabilidad mínima esperada
- Trazas por `requestId` para:
  - resultado de inserción de gabinete,
  - resultado de inventario (`IdRegistroProduccionDocumental`),
  - resultado de índice lógico (`id_cert_indice_expediente`),
  - resultado de actualización XML de expediente.

## Resultado funcional esperado
- Si toda la transacción DB es exitosa: quedan consistentes `system1`, `disco_detalle`, gabinete dinámico, inventario y (si aplica) índice lógico.
- Si falla fase física post-commit: ejecuta compensación DB según política vigente.
