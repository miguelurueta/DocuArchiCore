# SCRUM-193 - Regresión Legacy Compensación DB

## Comparativo VB Legacy vs C#

| Comportamiento | VB Legacy | C# SCRUM-193 | Estado |
|---|---|---|---|
| Error físico post-commit mantiene error raíz | Sí | Sí (relanza `StoragePhysicalException`) | Cumple |
| Reversión lógica DB tras fallo físico | No formalizada por servicio dedicado | Sí (`IStorageDbCompensationService`) | Mejora controlada |
| Reversión `system1` (proxid/numcarp) | No | No | Cumple |
| Reversión de efectos documentales (`gabinete`, `inventario`, `log`) | Parcial/manual según flujo | Sí, automatizada por plan | Cumple con desviación aprobada |
| Marcador de inventario por fallo físico | N/A específico | `ESTADO_ELIMINA_ERROR_ALMACENAMIENTO=1` | Cumple |
| Trazabilidad explícita de compensación | Baja | Alta (`OK|PARTIAL|FAILED`) | Mejora controlada |

## Conclusión de Regresión
- No se altera el comportamiento funcional de éxito.
- En fallo físico post-commit, C# mejora trazabilidad y recuperación sin romper invariantes legacy.
- La diferencia es intencional y orientada a operación segura.
