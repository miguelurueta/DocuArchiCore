# SCRUM-168 Pruebas Gabinete Inventario

## Matriz unitaria objetivo
- Validaciones de `GabineteInsertModel`:
  - nombre de gabinete vacio o invalido.
  - columnas dinamicas vacias, invalidas o duplicadas.
  - id/folio/carpeta/tipo fuera de rango.
- Validaciones de `InventarioInsertModel`:
  - campos obligatorios invalidos.
  - truncado seguro de `FullText` si supera limite.
  - id generado invalido.
- Coordinador:
  - orden de fases (reserve -> gabinete -> inventario -> commit).
  - rollback si falla inventario.
  - no ejecutar inventario si falla gabinete.

## Matriz integracion objetivo
- Commit exitoso persiste ambas inserciones.
- Error posterior a gabinete revierte ambas tablas.
- Transaccion compartida en ambas operaciones.
- Validacion de identificadores dinamicos contra inyeccion.

## Matriz concurrencia objetivo
- Ejecuciones paralelas no deben colisionar `IdAlmacen`.
- Fallo de una transaccion no contamina otra.
- Locks mantienen consistencia de secuencia.

## Matriz QT/calidad objetivo
- Sin `SELECT *` en rutas criticas.
- Sin `ExecuteAsync/QueryAsync/ExecuteScalarAsync` directo en repositories (si aplica DapperCrudEngine).
- Logs sin datos sensibles (`FullText`, payload documental).

## Evidencia de la corrida SCRUM-168 (coordinador)
- En `DocuArchiCore` se completo y valido OpenSpec del cambio.
- `opsxj:orchestrate:publish` genero PRs satelite para Api/DTOs/Services/Models.
- `MiApp.Repository` no genero diff (`traceability_only`), por lo que las pruebas de insercion repository quedan condicionadas al PR de repository.

## Cobertura actual vs pendiente
- Cubierto en esta fase:
  - trazabilidad de arquitectura, alcance y gating de flujo orquestado.
  - publicacion de PRs satelite con cambios funcionales.
- Pendiente:
  - evidencia de pruebas repository especificas de insercion dinamica gabinete/inventario cuando exista diff real en `MiApp.Repository`.
