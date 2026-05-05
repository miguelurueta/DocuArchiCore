# SCRUM-186 — Implementación Detallada Logdocuarchi Workflow

## Modelos
- `StorageContext.IpTrans`
- `WorkflowStorageLogModel` (propiedades legacy: `IdTran`, `DescOp`, `USER_OPER`, etc.)
- `WorkflowStorageLogBuildResult`

## Services
- `IWorkflowStorageLogBuilder` ahora recibe:
  - `StorageContext`
  - `StorageIdentityModel`
  - `StorageNamingResult`
  - `StoragePhysicalPathModel`
- `WorkflowStorageLogBuilder`:
  - retorna `NO_WORKFLOW` si `IdTareaWorkflow <= 0`
  - arma `RutDocu` con `RutaFinal + NombreArchivoPrincipal`
  - construye `Campos` en formato legacy `|valor1|valor2`
  - usa tipología por descripción (`Trd.NombreTipoDocumento`)
- `IWorkflowStorageLogService` / `WorkflowStorageLogService`:
  - encapsula decisión e inserción
  - no inserta cuando `ShouldInsert=false`

## Repository
- `WorkflowStorageLogRepository` actualizado a columnas y nombres legacy:
  - `id_tran`, `desc_op`, `USER_OPER`, `DATE_TRANS`, `RUT_DOCU`, `MODULO_REGISTRO`, `GABINETE`, `CAMPOS`, `IP_TRANS`, `HORA_REGISTRO`, `RADICADO`, `ID_TAREA_WF`, `ID_RUTA_WF`, `USER_PROPIETARIO`, `TIPOLOGIA_DOCUMENTAL`
- validaciones de modelo reforzadas.

## Coordinator
- `StorageTransactionCoordinator` ahora usa:
  - `IWorkflowStorageLogService`
  - `IStorageNamingService`
  - `IStoragePhysicalPathService`
- la inserción del log ocurre dentro de la transacción activa.

## API
- `AlmacenamientoDocumentalController`:
  - usa `IIpHelper` (`IpHelperL`) para IP real
  - pasa IP al use case
- `AlmacenarDocumentoUseCase`:
  - firma extendida con `ipTrans`
  - asigna `StorageContext.IpTrans`

