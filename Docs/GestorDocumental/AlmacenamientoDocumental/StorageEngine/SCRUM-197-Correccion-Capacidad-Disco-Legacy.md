# SCRUM-197 - Corrección Capacidad de Disco Legacy

## Contexto

Se detectó falla transaccional en almacenamiento por dependencia obligatoria de
`disco_detalle.ESTADO_DISCO`:

- Error observado: `Unknown column 'ESTADO_DISCO' in 'field list'`.
- Causa raíz: en ambientes productivos legacy la columna no existe.
- Requisito funcional: mantener paridad VB (`Numero_Imagenes`) basada en
  `TAMDISC + NUMERO_IMAGENES`.

## Cambios implementados

1. `StorageDiskQuotaRepository.LockDiskStatusAsync`:
- Intenta lectura con `ESTADO_DISCO`.
- Si falla por columna desconocida, reintenta en la misma transacción usando
  `'' AS EstadoDisco`.
- Conserva `FOR UPDATE` y filtros `disco + gabinete`.

2. `StorageDiskQuotaPolicy`:
- Fuente primaria de bloqueo: regla legacy de umbrales.
- Reglas:
  - `tamdisc > 572523149` y `numero_imagenes > 80000` => bloquea.
  - `tamdisc < 572523149` y `numero_imagenes > 7500` => bloquea.
  - `tamdisc == 572523149` => no bloquea por umbral.
- Reglas legacy de sincronización:
  - `numero_imagenes null` => error `estado null`.
  - `numero_imagenes == 0` => error de no sincronizado.
- Compatibilidad opcional:
  - si `EstadoDisco == "SL"` => bloquea.

3. `StorageIdentityAllocator`:
- Envía `systemRow.TamDisc` a la policy.
- Mejora trazabilidad de log con `tamDisc`, `estadoDisco` y `numeroImagenes`.

4. `DiskQuotaStatusModel`:
- `EstadoDisco` ahora es opcional.
- Se adiciona `NumeroImagenesIsNull` para paridad de validación legacy.

## Matriz de decisiones

| Escenario | Fuente principal | Resultado |
|---|---|---|
| Schema sin `ESTADO_DISCO` | `tamdisc + numero_imagenes` | Funciona sin error de columna |
| Schema con `ESTADO_DISCO` | `tamdisc + numero_imagenes` (+ refuerzo `SL`) | Compatibilidad con ambos modelos |
| `tamdisc == 572523149` | Regla legacy exacta | No activa bloqueo por umbral |

## Evidencia de pruebas

Pruebas ejecutadas:

`dotnet test tests/TramiteDiasVencimiento.Tests --filter "StorageDiskQuotaPolicyTests|StorageDiskQuotaRepositoryTests|StorageIdentityAllocatorTests"`

Resultado:
- `14/14` pruebas superadas.

Cobertura incluida:
- umbrales legacy (`80000`, `7500`, borde `572523149`);
- `numero_imagenes` null y `0`;
- compatibilidad opcional `EstadoDisco == "SL"`;
- fallback de repositorio ante `Unknown column 'ESTADO_DISCO'`.
