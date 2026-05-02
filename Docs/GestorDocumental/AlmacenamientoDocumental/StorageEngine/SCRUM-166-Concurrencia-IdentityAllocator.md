# SCRUM-166 Concurrencia IdentityAllocator

## Estrategia de locking
- Lock pesimista de `system1` por gabinete (`FOR UPDATE`).
- Lock pesimista de `disco_detalle` por gabinete/disco (`FOR UPDATE`).
- Orden fijo: `system1` -> `disco_detalle`.

## Control de colision en update
- Update de `system1` condicionado por `nombre` y `previousProxId`.
- Si `rows != 1`, se considera conflicto de concurrencia y se aborta.

## Riesgos de deadlock
- Si otro flujo usa orden inverso de lock, existe riesgo de espera circular.
- Mitigacion definida: estandarizar orden unico de lock en prompts 5/6.

## Pruebas de concurrencia sugeridas
- Dos hilos reservando sobre mismo gabinete:
  - resultado esperado: proxid monotono sin duplicados.
- Un hilo con disco en `SL`:
  - resultado esperado: rollback y no avance de proxid.
- Contencion alta:
  - medir tiempo promedio de lock y tasa de conflictos.
