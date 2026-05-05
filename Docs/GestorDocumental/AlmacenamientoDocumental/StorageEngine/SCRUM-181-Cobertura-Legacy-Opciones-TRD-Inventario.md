# SCRUM-181 - Cobertura Legacy Opciones TRD/Inventario

| Regla legacy VB | Componente nuevo | Estado |
|---|---|---|
| Verifica opcion inventario documental por gabinete | `IStorageOptionsResolver` + `StorageOptionsValidator` | Cumple |
| Si aplica inventario: usuario gestion obligatorio | `StorageOptionsValidator` (`INV_USER_REQUIRED`) | Cumple |
| Si aplica inventario: empresa obligatoria | `StorageOptionsValidator` (`INV_EMPRESA_REQUIRED`) | Cumple |
| Verifica opcion aplica TRD | `IStorageOptionsResolver` + `TrdRulesValidator` | Cumple |
| IDs TRD invalidos | `TrdRulesValidator` (`TRD_INVALID_*`) | Cumple |
| Verifica opcion unidad/expediente | `IStorageOptionsResolver` + `ExpedienteUnidadRulesValidator` | Cumple |
| Expediente/unidad exige clase documento | `ExpedienteUnidadRulesValidator` (`EXP_CLASE_REQUIRED`, `UNI_CLASE_REQUIRED`) | Cumple |

## Notas
- Validaciones de estado profundo de expediente/unidad se mantienen en fases transaccionales posteriores.
- El alcance de SCRUM-181 se enfoco en paridad de reglas previas y estabilidad de contratos.
