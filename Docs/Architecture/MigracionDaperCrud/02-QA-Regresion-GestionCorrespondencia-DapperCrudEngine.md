# PROMPT ARQUITECTONICO - Ticket QA-01

# Pruebas de regresion - GestionCorrespondencia migrado a DapperCrudEngine

## OBJETIVO
Validar que la migracion del modulo `GestionCorrespondencia` conserva comportamiento funcional y contratos.

## ALCANCE
Cobertura de:
- `SolicitaEstructuraRespuestaIdTareaAsync(...)`
- `SolicitaCorreoElectronicoRemitenteAsync(...)`

## PRUEBAS
1. Unitarias sobre service/controller asociados.
2. Focalizadas sobre estado `success/empty/error`.
3. Compilacion cruzada de proyectos impactados.

## RESTRICCIONES
- No cambiar contratos publicos.
- No convertir pruebas en mocks irreales.

## ENTREGABLE
- evidencia de tests ejecutados
- riesgos/regresiones
- funciones validadas y pendientes
