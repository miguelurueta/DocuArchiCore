## Context

- Jira issue key: SCRUM-200
- Jira summary: INTEGRACION-GABINETE-DINAMICO-INDICE-ELECTRONICO
- Jira URL: https://contasoftcompany.atlassian.net/browse/SCRUM-200

## Context Reference

- openspec/context/multi-repo-context.md
- openspec/context/OPSXJ_BACKEND_RULES.md

## Problem Statement

El flujo actual del Storage Engine reserva identidad y crea archivos fisicos, pero no completa paridad transaccional:

- Se incrementa `system1.PROXID`.
- Se actualiza `disco_detalle`.
- Se crean `DIG/FXL`.
- No se inserta el registro del gabinete dinamico (`context.NombreGabinete`, por ejemplo `contabil`).
- No se inserta `ra_cert_indice_expediente` en el flujo principal cuando aplica expediente.
- En inventario pueden quedar descriptores TRD nulos (`NOMBRE_AREA_DEPARTAMENTO`, `SERIE_DOCUMENTO`, `SUBSERIE_DOCUMENTO`).

## Goals

1. Insertar siempre el registro del gabinete dinamico dentro de la transaccion principal antes de `Commit`.
2. Insertar indice electronico logico (`ra_cert_indice_expediente`) cuando aplique expediente y existan prerequisitos.
3. Corregir llenado descriptivo TRD en `registro_producion_documental` con fallback de metadata.
4. Mantener atomicidad y compensacion coherente entre DB, FS y XML.

## Non-Goals

1. Cambiar contratos HTTP del controller de almacenamiento.
2. Hardcodear gabinetes o extensiones.
3. Introducir una transaccion separada para gabinete/indice.

## Current Flow Summary

1. `StorageIdentityAllocator.ReserveAsync` bloquea/actualiza `system1`.
2. `StorageTransactionCoordinator` bloquea/actualiza `disco_detalle`.
3. Inserta inventario/workflow condicional.
4. Hace `Commit`.
5. Fase fisica copia archivo, crea XML y opcionalmente intenta XML de expediente.

Gap: falta insercion de gabinete dinamico y falta insercion de indice electronico logico dentro de la misma transaccion.

## Target Transaction Sequence

1. Reserve identity (`system1`).
2. Lock/update disk quota (`disco_detalle`).
3. Insert gabinete dinamico (obligatorio).
4. Insert inventario documental (condicional).
5. Execute expediente/unidad legacy (condicional).
6. Insert indice electronico logico (condicional expediente + prerequisitos).
7. Insert workflow log (condicional).
8. Commit.

## Design Decisions

1. `IdAlmacen` sera la identidad transversal para gabinete/inventario/workflow/expediente/XML/compensacion.
2. El insert de gabinete dinamico falla rapido y fuerza rollback total.
3. `DBT` para gabinete se resuelve desde `DA_EXTENSION.ESTADO_NORMAL` (normalizando extension con/sin punto y case-insensitive).
4. Si el request no trae nombres TRD descriptivos, se resuelven por metadata antes de insertar inventario.
5. El indice electronico solo se inserta con expediente valido e `IdRegistroProduccionDocumental > 0`.

## Data/Repository Impact

- `MiApp.Services/.../Transaction/StorageTransactionCoordinator.cs`
- `MiApp.Repository/.../Gabinete/IGabineteStorageRepository.cs`
- `MiApp.Services/.../Inventario/InventarioDocumentalBuilder.cs` y/o servicios de resolucion TRD
- `MiApp.Services/.../Expediente/*` + `MiApp.Repository/.../IndiceElectronico/IIndiceElectronicoRepository.cs`
- `MiApp.Services/.../Compensation/IStorageDbCompensationService.cs`
- `MiApp.Repository/.../Compensation/IStorageDbCompensationRepository.cs`

## Failure and Compensation Strategy

1. Si falla cualquier paso antes de `Commit`, rollback DB total.
2. Si falla fase fisica post-commit, ejecutar compensacion DB completa:
   - Eliminar fila de gabinete dinamico.
   - Anular inventario documental.
   - Eliminar workflow log.
   - Revertir cuota disco/folios.
   - Revertir expediente/unidad.
   - Revertir indice electronico (delete o anulacion segun politica existente).

## Observability

Agregar trazas estructuradas por `requestId`:

- inicio/fin insert gabinete (`idAlmacen`, `gabinete`).
- inicio/fin insert indice (`idRegistroProduccion`, `idExpediente`).
- decisiones de rama condicional (aplica/no aplica inventario, expediente, workflow).
- resultado de compensacion por cada paso.

## Validation Rules

1. Extension mapping: soportar `PDF` y `.PDF`, sin hardcode de tipos.
2. Campos dinamicos de gabinete: validar tipo, longitud y nulabilidad con metadata existente.
3. Campos descriptivos TRD en inventario no deben persistir nulos cuando hay IDs TRD informados.

## Test Strategy

1. Unit tests:
   - insert gabinete obligatorio.
   - mapeo `DBT` desde `DA_EXTENSION`.
   - llenado TRD descriptivo + fallback.
   - guardas del indice electronico.
2. Integration tests:
   - happy path `contabil` con fila DB + archivos DIG/FXL.
   - expediente habilitado con `ra_cert_indice_expediente`.
   - fallos transaccionales con rollback total.
   - fallo fisico post-commit con compensacion efectiva.

## Risks

1. Metadatos TRD incompletos en ambientes legacy pueden impedir fallback descriptivo.
2. Politica de compensacion para indice electronico puede variar por ambiente (delete vs anulacion).
3. Validaciones estrictas de metadata dinamica pueden revelar datos historicos inconsistentes.
