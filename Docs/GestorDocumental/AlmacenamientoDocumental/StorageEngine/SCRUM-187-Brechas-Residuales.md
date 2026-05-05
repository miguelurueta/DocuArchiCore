# SCRUM-187 — Brechas Residuales

## Brechas identificadas
1. Falta corrida E2E completa con Docker/Testcontainers y dataset legacy consolidado.
2. Falta validación concurrente real (2 y 5 hilos) contra MySQL con `FOR UPDATE`.
3. Falta lectura automática de snapshots legacy históricos para comparación directa VB runtime.

## Impacto
- No bloquea validación unitaria/contractual de paridad.
- Sí bloquea una certificación final de equivalencia "cero brechas" para Go productivo estricto.

## Plan de cierre
1. Ejecutar suite en runner con Docker habilitado y semillas de tablas `system1`, `disco_detalle`, gabinete dinámico e inventario.
2. Registrar resultados por escenario en la matriz de paridad.
3. Completar evidencia de concurrencia real y actualizar estado a `CUMPLE` o `NO CUMPLE`.
