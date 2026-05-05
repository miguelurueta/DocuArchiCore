# SCRUM-183 - Regresion Legacy Inventario Documental

## Comparativo VB vs C#

| Comportamiento legacy VB | Estado C# SCRUM-183 |
|---|---|
| Inserta inventario solo si opcion inventario aplica | Implementado |
| Falla si opcion activa y faltan datos de inventario | Implementado |
| Usa numero folios del flujo de almacenamiento | Implementado (PhysicalMetadata) |
| Usa tamano/formato para inventario | Implementado |
| Usa radicado y segundo nombre documental | Implementado |
| Usa TRD y expediente/unidad si vienen informados | Implementado |
| Retorna id de registro de produccion | Implementado |

## Riesgo residual
- Algunas columnas descriptivas dependen de datos opcionales del request (nombres TRD/expediente/unidad). Si no se envian, se persisten `NULL`.
