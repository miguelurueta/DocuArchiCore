# SCRUM-172 Cobertura Legacy Workflow Log

## Mapeo logdocuarchi -> modelo nuevo

| Legacy `logdocuarchi` | `WorkflowStorageLogModel` |
|---|---|
| `id_tran` | `IdAlmacen` |
| `desc_op` | `DescripcionOperacion` |
| `USER_OPER` | `UsuarioOperacion` |
| `DATE_TRANS` | `FechaTransaccion` |
| `RUT_DOCU` | `RutaDocumento` |
| `MODULO_REGISTRO` | `ModuloRegistro` |
| `GABINETE` | `NombreGabinete` |
| `CAMPOS` | `Campos` |
| `IP_TRANS` | `IpTransaccion` |
| `HORA_REGISTRO` | `HoraRegistro` |
| `RADICADO` | `Radicado` |
| `ID_TAREA_WF` | `IdTareaWorkflow` |
| `ID_RUTA_WF` | `IdRutaWorkflow` |
| `USER_PROPIETARIO` | `UsuarioPropietario` |
| `TIPOLOGIA_DOCUMENTAL` | `TipologiaDocumental` |

## Compatibilidad funcional
- La escritura se mantiene condicionada a `IdTareaWorkflow > 0`.
- Se conserva trazabilidad por `IdAlmacen`, gabinete, ruta logica, radicado y workflow.
- Mantiene semantica transaccional: falla de log implica rollback total del flujo storage.

## Regla DapperCrudEngine
- El repositorio debe usar `DapperCrudEngine + QueryOptions`.
- No se permite SQL concatenado ni Dapper directo desde repositories.

