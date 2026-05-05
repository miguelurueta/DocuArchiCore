# SCRUM-186 — Regresión Legacy Logdocuarchi Workflow

## Equivalencia VB vs C#
| VB legacy | C# SCRUM-186 |
|---|---|
| `id_tran = idal` | `IdTran = identity.IdAlmacen` |
| `desc_op = Registra` | `DescOp = Registra` |
| `USER_OPER = useral` | `UserOper = context.Usuario` |
| `DATE_TRANS = date1al` | `DateTrans = UtcNow.Date` |
| `RUT_DOCU = ruta DIG` | `RutDocu = RutaFinal + NombreArchivoPrincipal` |
| `MODULO_REGISTRO = WORKFLOW` | `ModuloRegistro = WORKFLOW` |
| `GABINETE` | `Gabinete = Command.NombreGabinete` |
| `CAMPOS = fultex_log` | `Campos = |valor1|valor2|...` |
| `IP_TRANS = ip_host_name` | `IpTrans = IIpHelper.ObtenerDireccionIP(HttpContext)` |
| `HORA_REGISTRO` | `HoraRegistro = HH:mm:ss` |
| `RADICADO` | `Radicado = Inventario.Radicado` |
| `ID_TAREA_WF` | `IdTareaWorkflow` |
| `ID_RUTA_WF` | `IdRutaWorkflow` |
| `USER_PROPIETARIO` | `UserPropietario = context.Usuario` |
| `TIPOLOGIA_DOCUMENTAL` descripción | `TipologiaDocumental = Trd.NombreTipoDocumento` |

## Regla de activación
- VB: inserta solo si `id_tarea_workflow <> 0`
- C#: `ShouldInsert = false` cuando `IdTareaWorkflow <= 0`

## Riesgo mitigado
- auditoría incompleta por IP vacía
- tipología por ID en vez de descripción
- campos no legacy
- ruta de log no alineada con naming/ruta final

