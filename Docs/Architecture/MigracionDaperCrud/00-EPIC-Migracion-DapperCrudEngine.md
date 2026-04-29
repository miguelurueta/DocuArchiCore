# Migracion DapperCrudEngine - Plan de Tickets

## Objetivo
Corregir deuda tecnica de repositorios que consumen datos sin `DapperCrudEngine` en `MiApp.Repository/Repositorio`, sin cambiar contratos publicos ni comportamiento funcional.

## Lotes por modulo
1. GestionCorrespondencia (BE + QA)
2. GestorDocumental/Editor (BE + QA)
3. Radicador/Tramite (BE + QA)
4. Workflow/RutaTrabajo (BE + QA)

## Regla obligatoria por ticket
- Especificar ruta y funcion intervenida.
- Refactor interno unicamente (contratos HTTP/publicos intactos).
- Validacion por lote: compilacion + tests focalizados + riesgos/regresiones.
- Entregable de cierre por lote:
  - funciones migradas
  - funciones pendientes
  - decisiones tecnicas
  - plan de cierre
