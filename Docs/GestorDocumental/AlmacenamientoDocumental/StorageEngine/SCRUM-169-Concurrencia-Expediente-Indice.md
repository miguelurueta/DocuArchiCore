# SCRUM-169 Concurrencia Expediente Indice

## Estrategia
- Lock pesimista con `FOR UPDATE` para expediente y unidad.
- Orden de lock fijo: expediente -> unidad.
- Updates con filtro de valor previo (control optimista adicional).

## Riesgos
- Deadlock si otro flujo usa orden inverso de locks.
- Contencion en alta concurrencia sobre el mismo expediente.

## Mitigaciones
- Mantener orden de lock uniforme en todos los flujos storage.
- Mensajes de error específicos de concurrencia para reintento controlado.
- Monitoreo de conflictos en updates con `rows != 1`.
