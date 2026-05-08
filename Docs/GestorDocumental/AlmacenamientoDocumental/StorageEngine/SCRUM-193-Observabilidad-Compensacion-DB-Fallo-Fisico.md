# SCRUM-193 - Observabilidad Compensación DB

## Eventos de Log
- Inicio/fin de compensación DB con `requestId`, `idAlmacen`, `estado`, `duracionMs`.
- Log por paso (`DISK_USAGE_REVERT`, `EXPEDIENTE_REVERT`, `UNIDAD_REVERT`, `INVENTARIO_ANNUL`, `WORKFLOW_LOG_DELETE`, `GABINETE_DELETE`).
- Warning cuando auditoría de compensación no está disponible.

## Correlación
- Llave principal: `requestId`.
- Correlación secundaria: `idAlmacen`, `idRegistroProduccionDocumental`.

## Regla Operativa
- Nunca se oculta la excepción física original.
- Resultado de compensación se registra como `OK`, `PARTIAL` o `FAILED`.
