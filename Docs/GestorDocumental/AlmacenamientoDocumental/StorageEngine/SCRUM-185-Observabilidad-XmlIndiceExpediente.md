# SCRUM-185 — Observabilidad XML Índice de Expediente

## Eventos relevantes
- Inicio de actualización XML índice.
- Ruta XML resuelta desde configuración legacy.
- Resultado de escritura XML (`UPDATED`, `NO_EXPEDIENTE`, `POST_COMMIT_INCONSISTENCY`).

## Contexto mínimo de log
- `requestId`
- `nombreGabinete`
- `idExpediente`
- `idRegistroProduccionDocumental`
- `estadoExpedienteElectronico`
- `estadoResultadoXmlIndice`

## Manejo de errores
- Error de actualización XML índice:
  - no cancela almacenamiento físico principal,
  - se guarda en `StoragePhysicalStatusModel.InconsistenciaPostCommit`,
  - se marca `ExpedienteIndiceXmlResult.Estado = POST_COMMIT_INCONSISTENCY`.

## Datos excluidos de logs
- contenido documental
- fulltext
- payload binario
- datos sensibles del expediente

