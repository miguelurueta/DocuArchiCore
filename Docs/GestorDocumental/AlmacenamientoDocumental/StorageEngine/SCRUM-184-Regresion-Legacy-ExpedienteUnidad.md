# SCRUM-184 — Regresión Legacy Expediente/Unidad

## Matriz VB vs C#

| Regla | Legacy VB | C# objetivo |
|---|---|---|
| Expediente y unidad como casos separados | Sí | Sí |
| Requiere ambos IDs | No | No |
| Permite ambos IDs simultáneos | No (ambigüedad funcional) | No (error explícito) |
| Clase documental obligatoria | Sí (si hay expediente/unidad) | Sí |
| Valida estado expediente | Sí (`ESTADO_EXPEDIENTE=1`) | Sí |
| Lee estado expediente electrónico | Sí | Sí |
| Folios expediente | `NUMERO_ELECTRONICO_CONTENIDO` | `NUMERO_ELECTRONICO_CONTENIDO` |
| Folios unidad digitalizada | `NUMERO_DIGITALIZADO_CONTENIDO` | `NUMERO_DIGITALIZADO_CONTENIDO` |
| Folios unidad electrónica | `NUMERO_ELECTRONICO_CONTENIDO` | `NUMERO_ELECTRONICO_CONTENIDO` |
| Campo incorrecto unidad | No usa `NUMERO_FOLIO_UNIDAD_CONSERVACION` | No usar |
| Tipo unidad documental | 2=expediente / 1=unidad | 2=expediente / 1=unidad |
| Lock transaccional | `FOR UPDATE` | `FOR UPDATE` |

## Casos de regresión obligatorios
- Expediente activo incrementa folios electrónicos.
- Expediente cerrado bloquea operación.
- Unidad digitalizada incrementa contador digitalizado.
- Unidad electrónica incrementa contador electrónico.
- Ambigüedad expediente+unidad devuelve error.
- Sin clase documental devuelve error.

## Riesgo si no se cumple
- Conteo archivístico inconsistente.
- Pérdida de paridad legal/operativa con VB.
- Errores de inventario e índice posteriores.
