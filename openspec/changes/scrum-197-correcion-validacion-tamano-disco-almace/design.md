## Context

- Jira issue key: `SCRUM-197`
- Jira summary: `CORRECION-VALIDACION-TAMANO-DISCO-ALMACENAMIENTO`
- Change: `scrum-197-correcion-validacion-tamano-disco-almace`

References:
- `openspec/context/multi-repo-context.md`
- `openspec/context/OPSXJ_BACKEND_RULES.md`

## Problem

En la reserva de identidad transaccional, el repositorio de cuota de disco selecciona
`ESTADO_DISCO` desde `disco_detalle`. En ambientes legacy/productivos esa columna no existe,
por lo que la operacion falla antes de validar capacidad.

La paridad funcional requerida viene de legacy VB (`Numero_Imagenes`):
- Fuente primaria: `TAMDISC` (system1) + `NUMERO_IMAGENES` (disco_detalle)
- Regla de bloqueo:
  - `tamdisc > 572523149` y `numero_imagenes > 80000` => bloquear (`SL`)
  - `tamdisc < 572523149` y `numero_imagenes > 7500` => bloquear (`SL`)
  - `tamdisc == 572523149` => no activar `SL` por umbral
- Si `numero_imagenes` es null o `0` => error de disco no sincronizado.

## Goals

1. Eliminar dependencia obligatoria de `ESTADO_DISCO`.
2. Replicar paridad legacy en validacion de capacidad.
3. Conservar compatibilidad con ambientes que si tienen `ESTADO_DISCO`.
4. Mantener validacion dentro de transaccion con locks `FOR UPDATE`.

## Non-Goals

- Cambiar contratos HTTP/DTOs publicos.
- Introducir migracion obligatoria de schema.
- Redefinir umbrales legacy.

## Architecture

### Current flow

`StorageTransactionCoordinator` -> `StorageIdentityAllocator` ->
`StorageDiskQuotaRepository.LockDiskStatusAsync` -> `StorageDiskQuotaPolicy.ValidateDiskAvailable`.

### Target flow

1. Bloquear `system1` (`FOR UPDATE`) y obtener `tamdisc`.
2. Bloquear `disco_detalle` (`FOR UPDATE`) y obtener `numero_imagenes`/`numpag_carp`.
3. Evaluar policy de capacidad con regla legacy (`tamdisc + numero_imagenes`).
4. Si bloquea, lanzar `StorageTransactionException` antes de updates/commit.
5. Si permite, continuar flujo actual.

## Data contract adjustments

- `DiskQuotaStatusModel` debe permitir operar sin `ESTADO_DISCO` obligatorio.
- `StorageDiskQuotaRepository.LockDiskStatusAsync` no debe fallar cuando esa columna no exista.
- Si la columna existe, puede mapearse opcionalmente para refuerzo (`SL` explicito).

## Decision matrix (with/without ESTADO_DISCO)

1. Schema sin `ESTADO_DISCO`:
- Validacion se resuelve solo por regla legacy.
- Flujo debe continuar sin error de columna.

2. Schema con `ESTADO_DISCO`:
- Se mantiene regla legacy como fuente principal.
- `EstadoDisco == "SL"` puede reforzar bloqueo.

## Error behavior

- `numero_imagenes` null:
  - `El disco {disc} no esta sincronizado para alamcenar contacte a su administrador estado null`
- `numero_imagenes == 0`:
  - `El disco {disc} no esta sincronizado para alamcenar contacte a su administrador`
- Umbral excedido:
  - `Disco {disc} Sobrepaso el limite de capacidad`

## Testing strategy

Unit tests:
- umbral alto (`>80000`) con `tamdisc > 572523149` bloquea.
- umbral bajo (`>7500`) con `tamdisc < 572523149` bloquea.
- `tamdisc == 572523149` no bloquea por umbral.
- `numero_imagenes` null y `0` retornan error esperado.
- si existe `EstadoDisco == SL`, bloquea.

Integration tests:
- ambiente sin `ESTADO_DISCO` no rompe lectura transaccional.
- ambiente con `ESTADO_DISCO` mantiene comportamiento.
- lock transaccional (`FOR UPDATE`) se conserva.

## Risks

- Riesgo de regresion funcional en cuota de disco si se altera la regla de umbrales.
- Riesgo de inconsistencia de mensajes si no se homologa con legacy.
- Riesgo de concurrencia si se elimina/relaja lock.

Mitigaciones:
- pruebas de paridad con matriz explicita de casos.
- mantener orden de lock y validacion antes de commit.
