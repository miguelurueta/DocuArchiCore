# SCRUM-190 - Arquitectura Cierre Orquestador

## Objetivo
Cerrar la brecha funcional del runtime de `DocumentStorageOrchestrator` para que el flujo C# ejecute de forma real las fases de validacion, metadata, transaccion y fase fisica, con manejo de errores tipados.

## Alcance Arquitectonico
- Componente: `DocumentStorageOrchestrator`.
- Capa: `MiApp.Services` (Application/Engine).
- No se modifica:
  - SQL legacy.
  - estructura de tablas.
  - contratos API.

## Decisiones Arquitectonicas
1. El orquestador pasa de stub a coordinador real de fases.
2. Se conserva separacion por responsabilidades:
- `IStorageValidationPipeline`
- `IStorageDocumentMetadataAnalyzer`
- `IStorageTransactionCoordinator`
- `IStoragePhysicalPhaseExecutor`
3. Se formaliza manejo de error por jerarquia:
- `StorageValidationException`
- `StoragePhysicalException`
- `StorageTransactionException`
- `StorageException` (base)
4. Se agrega `try/catch` central para trazabilidad y control de errores no tipados.

## Flujo Objetivo
1. Validar `StorageContext` y `RequestId`.
2. Ejecutar pipeline de validacion.
3. Resolver archivos temporales desde `ArchivoTemporalId`.
4. Analizar metadata fisica y validar paginas > 0.
5. Ejecutar coordinacion transaccional y validar `IdAlmacen > 0`.
6. Ejecutar fase fisica y validar `Completed`.
7. Retornar `AlmacenarDocumentoResult` final.

## Compatibilidad Legacy (Parcial)
- Alinea el comportamiento de orquestacion por fases antes de responder.
- Evita retorno exitoso con identidad invalida (`IdAlmacen = 0`), desviacion critica detectada previamente.
- Centraliza captura de errores como hacia el bloque de control general del legado.

## Observabilidad
Logs obligatorios en inicio, fallo y fin:
- `requestId`
- `nombreGabinete`
- `usuarioId`
- `idAlmacen`
- `estado`

## Riesgos Mitigados
- Exito falso por stub (`Pending`, `IdAlmacen=0`).
- Fallos silenciosos en fase fisica/transaccional.
- Ausencia de rastreo de excepciones no tipadas.

