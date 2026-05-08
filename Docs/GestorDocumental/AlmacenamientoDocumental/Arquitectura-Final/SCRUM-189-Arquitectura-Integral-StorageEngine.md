# SCRUM-189 — Arquitectura Integral StorageEngine

## 1. Contexto
- Sistema: `StorageEngineV2` para reemplazo progresivo de función VB `Almacenamiento`.
- Patrón objetivo: `Controller -> UseCase -> Orchestrator/Engine -> Validation -> Transaction -> Physical/XML -> Compensation`.
- Feature flag: `StorageEngineV2` (API).
- Repos de implementación:
  - `DocuArchi.Api`
  - `MiApp.Services`
  - `MiApp.Repository`
  - `MiApp.Models`
  - `MiApp.DTOs`
  - `DocuArchiCore` (tests/docs/openspec)

## 2. Requisitos Funcionales
1. Validar estructura de request, documentos y reglas por tipo de almacenamiento.
2. Resolver metadata física (`tamaño`, `formato`, `páginas`) antes de persistencia.
3. Leer e integrar preindex (`.txt` / `.xmls`) para flujos batch.
4. Reservar identidad documental (`PROXID`) con bloqueo transaccional.
5. Actualizar cuota de disco/carpeta (`disco_detalle`, `NUMPAG_CARP`).
6. Insertar inventario documental cuando `system1.INVENTARIO_DOCUMENTAL = 1`.
7. Ejecutar reglas expediente/unidad y actualizar folios.
8. Insertar índice electrónico (`ra_cert_indice_expediente`) y actualizar XML índice cuando aplique.
9. Persistir log workflow (`logdocuarchi`) cuando `IdTareaWorkflow > 0`.
10. Escribir archivos físicos DIG/FXL y aplicar compensación en fallos post-commit.
11. Ejecutar compensación DB post-fallo físico para revertir efectos lógicos persistidos.

## 3. Requisitos No Funcionales
- Consistencia transaccional DB: `IsolationLevel.Serializable`.
- Control de concurrencia y colisiones de identidad.
- Observabilidad por `requestId`, fase, duración y errores.
- Trazabilidad de decisiones legacy vs C#.
- Seguridad de rutas (resolución segura y control de traversal).

## 4. Restricciones Técnicas y de Negocio
- No tocar función VB legacy; referencia solo para paridad.
- No usar datos productivos en pruebas.
- Tablas dinámicas de gabinete deben respetar metadata real.
- Las opciones de `system1` gobiernan reglas (sin defaults falsos en diseño final).

## 5. Arquitectura Lógica
## Capa API
- `AlmacenamientoDocumentalController`
  - valida claims/feature flag
  - resuelve IP (`IIpHelper`)
  - delega en UseCase

## Capa Aplicación
- `AlmacenarDocumentoUseCase`
  - valida parámetros base
  - construye `StorageContext`
  - maneja `AppResponses` y traducción de excepciones

## Capa Engine/Orquestación
- `DocumentStorageOrchestrator`
  - coordina pipeline (actualmente con evolución incremental)
  - invoca analizador físico cuando está cableado
  - ejecuta compensación DB al capturar `StoragePhysicalException` post-commit

## Capa Validación
- `StorageValidationPipeline` + validadores:
  - estructura request
  - reglas básicas
  - preindex
  - campos obligatorios gabinete
  - opciones/trd/expediente/unidad

## Capa Transaccional
- `StorageTransactionCoordinator`
  - reserva identidad (`IStorageIdentityAllocator`)
  - lock/update cuota disco
  - inserta inventario (opcional)
  - ejecuta expediente/unidad (opcional)
  - inserta workflow log (opcional)

## Capa Física/XML
- `StoragePhysicalPhaseExecutor`
- `StoragePhysicalPathService`
- `StorageFileWriter`
- `StorageXmlWriter`
- `StorageCompensationManager`

## Capa Compensación DB
- `IStorageDbCompensationService` / `StorageDbCompensationService`
- `IStorageDbCompensationRepository` / `StorageDbCompensationRepository`
- modelos `StorageCompensationDbPlan`, `StorageCompensationDbResult`, `StorageCompensationDbStatus`

## Capa Repositorio
- Acceso SQL vía `IDapperCrudEngine` y repos especializados.

## 6. Integraciones Clave
1. `API -> UseCase`: contrato `AlmacenarDocumentoRequest/Response`.
2. `UseCase -> Orchestrator`: `StorageContext`.
3. `Orchestrator -> ValidationPipeline`: resultado `StorageValidationResult`.
4. `Orchestrator -> TransactionCoordinator`: `StorageTransactionResult`.
5. `Orchestrator -> PhysicalPhaseExecutor`: estado físico y XML.
6. `TransactionCoordinator -> Repository`: locks/inserts/updates.

## 7. Atomicidad y Consistencia
- DB: atomicidad fuerte en bloque transaccional `Serializable`.
- FileSystem/XML: no transaccional; se maneja compensación post-commit.
- Modelo final: consistencia eventual controlada para fase física.

## 8. Observabilidad
- Logs estructurados en:
  - validation start/end
  - transaction start/end
  - workflow/inventario/expediente toggles
  - rollback y compensación
- Campos base: `requestId`, `usuarioId`, `alias`, `gabinete`, `idAlmacen`, `duracionMs`.

## 9. Estado de Implementación vs Prompt 2-21
- Base funcional implementada y documentada en tickets SCRUM-163..188.
- Paridad extendida y suite de auditoría/paridad iniciada en SCRUM-187.
- SCRUM-189 consolida la vista arquitectónica, auditoría integral y criterio Go/No-Go.

## 10. Cierre Arquitectónico Integrado (SCRUM-192)
- Tipo de cierre: auditoría técnica integral (sin implementación funcional nueva).
- Fuente legacy auditada: `D:\imagenesda\GestorDocumental\promp\CORE-API\Almacenamiento\funcion-almacena-consolidad.txt`.
- Resultado de auditoría: **GO CONDICIONADO**.
- Evidencia de pruebas ejecutadas: `131` total, `129` OK, `2` SKIP (Docker), `0` FAIL.

## 10.1 Evolución Post-Cierre (SCRUM-193)
- Se añadió compensación lógica DB post-fallo físico en el orquestador.
- Se incorporó trazabilidad de compensación (`OK|PARTIAL|FAILED`) y duración.
- Se mantiene invariante legacy: no revertir `system1.proxid/numcarp/NUMPAG_CARP`.
- Se conserva y relanza el error físico original como causa raíz.

## 11. Hallazgos de Cierre
- Paridad funcional crítica VB -> C# sin brechas clasificadas como `NO CUMPLE` crítico.
- Persistencia transaccional y fase física están separadas correctamente con compensación post-commit.
- Observabilidad base cumple para `requestId`, `idAlmacen` y trazas por fase.
- Brecha operativa residual: con `StorageEngineV2=false` no existe fallback legacy activo y hoy se retorna error controlado.

## 12. Condiciones para GO Pleno
1. Habilitar adapter/fallback legacy operativo para operación segura cuando `StorageEngineV2=false`.
2. Ejecutar suite de paridad completa en entorno con Docker y anexar evidencia de concurrencia e integración.

## 13. Artefactos Oficiales de Cierre SCRUM-189
- `SCRUM-189-Matriz-Paridad-StorageEngine.md`
- `SCRUM-189-Pruebas-E2E-StorageEngine.md`
- `SCRUM-189-Riesgos-Desviaciones-Aprobadas.md`
- `SCRUM-189-Go-NoGo-StorageEngine.md`
- `SCRUM-189-Metadata.md`
