# SCRUM-188 - Regresion Legacy Opciones System1

## Matriz VB vs C#

| Legacy VB | C# objetivo | Estado |
|---|---|---|
| `VerificaOpcionAplicarInventarioDocumental` | Resolver opcion `AplicaInventarioDocumental` desde `system1` | Cumplido (consolidado) |
| `VerificaOpcionAplicarTablaRetencion` | Resolver opcion `AplicaTrd` desde `system1` | Cumplido (consolidado) |
| `Verfica_opcion_seleccion_unidad` | Resolver opcion `AplicaUnidadConservacion` desde `system1` | Cumplido (consolidado) |
| Reglas condicionales por opcion activa | Validadores de opciones en pipeline | Cumplido (consolidado) |
| Decision de operaciones por opcion | `StorageTransactionCoordinator` condicionado por opciones | Cumplido (consolidado) |

## Brechas en SCRUM-188
- No se detectan brechas nuevas en este repositorio.
- SCRUM-188 no reabre implementacion funcional; centraliza evidencia de continuidad.

## Riesgo residual
- Riesgo principal: divergencia por cambios paralelos fuera del pipeline.
- Mitigacion: mantener pruebas de regresion y una sola fuente de verdad funcional por capability.
