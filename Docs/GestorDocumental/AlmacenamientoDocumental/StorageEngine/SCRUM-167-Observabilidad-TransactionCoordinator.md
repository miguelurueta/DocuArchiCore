# SCRUM-167 Observabilidad TransactionCoordinator

## Logs agregados
- Inicio de fase transaccional (`requestId`, `alias`, `usuarioId`, `gabinete`).
- Inicio y éxito de `disk quota update`.
- Workflow log omitido/no aplica.
- Workflow incompleto (`IdTareaWorkflow` nulo).
- Workflow no configurado en DI cuando se solicita.
- Workflow insertado (cuando extensión está disponible).
- Error transaccional y rollback.

## Campos recomendados
- `requestId`
- `alias`
- `usuarioId`
- `nombreGabinete`
- `disco`
- `idAlmacen`
- `diskUsageUpdated`
- `workflowLogInserted`
- `duracionMs`
- `estado`

## Troubleshooting
- `Conflicto de concurrencia al actualizar disco_detalle`:
  - revisar filtros `disco/gabinete` y lock previo.
- `workflow informado pero repository no configurado`:
  - esperado hasta implementar Prompt 11.
- `Error ejecutando StorageTransactionCoordinator`:
  - revisar excepción interna y evidencia de rollback.
