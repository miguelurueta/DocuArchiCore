## 1. Discovery and Baseline

- [x] 1.1 Confirmar diagnostico: se actualiza `system1/disco_detalle` y se crean `DIG/FXL`, pero falta insert de gabinete dinamico.
- [x] 1.2 Confirmar que `IGabineteStorageRepository` existe pero no se ejecuta en `StorageTransactionCoordinator`.
- [x] 1.3 Confirmar que `IIndiceElectronicoRepository` existe pero no esta integrado en la secuencia transaccional principal.
- [x] 1.4 Confirmar dependencias de expediente/XML y condicion de `IdRegistroProduccionDocumental`.

## 2. Design and Spec Refinement

- [x] 2.1 Refinar `design.md` con secuencia objetivo, decisiones de atomicidad y estrategia de compensacion.
- [x] 2.2 Refinar `specs/jira-scrum-200/spec.md` con requisitos verificables para gabinete, DBT, TRD, indice y compensacion.
- [x] 2.3 Mantener referencia explicita a `openspec/context/OPSXJ_BACKEND_RULES.md`.

## 3. Implementation Tasks

- [x] 3.1 Integrar `IGabineteStorageRepository` en `StorageTransactionCoordinator` con insercion obligatoria pre-commit.
- [x] 3.2 Construir `GabineteInsertModel` con columnas base (`ID, DISC, PAG, DBT, IDEX, USER, DATE1, TIME1`) y campos dinamicos.
- [ ] 3.3 Resolver `DBT` desde `DA_EXTENSION.ESTADO_NORMAL` con normalizacion de extension (`PDF`/`.PDF`).
- [ ] 3.4 Ajustar flujo de inventario para poblar `NOMBRE_AREA_DEPARTAMENTO`, `SERIE_DOCUMENTO`, `SUBSERIE_DOCUMENTO` con fallback de metadata TRD.
- [x] 3.5 Integrar `IIndiceElectronicoRepository` + builder/calculator en flujo transaccional condicional de expediente.
- [x] 3.6 Asegurar orden transaccional final: reserva -> cuota -> gabinete -> inventario -> expediente/unidad -> indice -> workflow -> commit.
- [ ] 3.7 Extender `StorageTransactionResult`/logging para trazabilidad de insercion gabinete e indice.

## 4. Compensation and Consistency

- [ ] 4.1 Verificar/ajustar compensacion DB para eliminar registro de gabinete en fallo fisico post-commit.
- [ ] 4.2 Verificar/ajustar compensacion de inventario/workflow/cuota/expediente ya existente.
- [ ] 4.3 Definir y aplicar politica de compensacion para `ra_cert_indice_expediente` (delete o anulacion) consistente con arquitectura.

## 5. Tests

- [x] 5.1 Unit tests: insert gabinete obligatorio y rollback al fallar.
- [ ] 5.2 Unit tests: mapeo `DBT` desde `DA_EXTENSION` para extension con/sin punto y case-insensitive.
- [ ] 5.3 Unit tests: llenado TRD descriptivo con request y fallback metadata.
- [x] 5.4 Unit tests: reglas de insercion de indice electronico (aplica/no aplica/expediente inactivo).
- [ ] 5.5 Integration tests: happy path `contabil` con fila DB + `DIG/FXL`.
- [ ] 5.6 Integration tests: expediente valido inserta `ra_cert_indice_expediente` y mantiene consistencia.
- [ ] 5.7 Integration tests: fallo fisico post-commit dispara compensacion y no deja huerfanos.

## 6. Documentation

- [x] 6.1 Actualizar `Docs/GestorDocumental/AlmacenamientoDocumental/StorageEngine/SCRUM-171-Integracion-API-Frontend.md` con contrato/flujo vigente.
- [ ] 6.2 Actualizar documentos base de arquitectura final de SCRUM-189 con la nueva secuencia y matriz de paridad.
- [x] 6.3 Crear/actualizar documentacion especifica SCRUM-200 (arquitectura, implementacion, pruebas, observabilidad, regresion, metadata).

## 7. Validation Gate

- [x] 7.1 Ejecutar `openspec.cmd validate scrum-200-integracion-gabinete-dinamico-indice-ele`.
- [ ] 7.2 Confirmar tasks completas antes de `opsxj:orchestrate:publish`.
