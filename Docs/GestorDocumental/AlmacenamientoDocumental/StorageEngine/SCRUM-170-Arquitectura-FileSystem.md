# SCRUM-170 Arquitectura FileSystem

## Objetivo
Implementar la fase física del Storage Engine (copy + XML) fuera de la transacción DB, con compensación y hardening de rutas.

## Relación con prompts
- Prompt 6 (Transaction): la fase DB termina en `StorageTransactionCoordinator`.
- Prompt 9 (este cambio): capa física y consistencia eventual.
- Prompt 10 (API final): consumo del resultado físico (`NombreArchivoFinal`, estado `Completed`).

## Separación DB vs FS
- DB: reservación e inserciones transaccionales.
- FS/XML: post-commit en `DocumentStorageOrchestrator` mediante `IStoragePhysicalPhaseExecutor`.

## Componentes
- `StoragePathResolver`: valida segmentos y evita path traversal.
- `StoragePlanBuilder`: construye plan físico determinista.
- `StorageFileWriter`: copia atómica con `.tmp` + `File.Move`.
- `StorageXmlBuilder`: modela metadata XML.
- `StorageXmlWriter`: escritura XML atómica.
- `StorageCompensationManager`: limpieza de artefactos en fallo.
- `StoragePhysicalPhaseExecutor`: orquesta copy -> xml -> compensation.

## Riesgos y mitigación
- Falla FS/XML post-commit DB: compensación best-effort.
- Path traversal: validación estricta de segmentos y root normalization.
- Corrupción parcial: escrituras temporales y move atómico.
