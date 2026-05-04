# SCRUM-178 — Cobertura Legacy Preindex Integración

| Legacy VB | Componente nuevo | Cobertura |
|---|---|---|
| Búsqueda preindex `.txt/.xmls` | `StoragePreindexResolver` + `StoragePreindexReader` | Parcial-alta |
| Requisito de preindex en batch | `PreindexValidator` | Completa |
| Mapeo posicional valores preindex | `StoragePreindexIntegrator` | Completa |
| No sobrescritura de valor manual | `StoragePreindexIntegrator` | Completa |
| Propagación a persistencia | `StorageContext.EffectiveCamposIndexacion` + Transaction/XML/Workflow builders | Completa |

## Diferencias controladas
- El legacy resolvía rutas físicas directas de carpeta operativa; el nuevo flujo limita resolución a ruta temporal controlada para seguridad.
- Se añadió fallback de candidato legacy normalizado para mejorar compatibilidad de nombre preindex.
