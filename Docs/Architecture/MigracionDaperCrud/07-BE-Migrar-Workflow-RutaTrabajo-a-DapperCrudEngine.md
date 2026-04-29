# PROMPT ARQUITECTONICO - Ticket BE-04

# Migrar Workflow/RutaTrabajo a DapperCrudEngine

## OBJETIVO
Estandarizar acceso a datos en `Workflow/RutaTrabajo` con `DapperCrudEngine` manteniendo contratos y comportamiento funcional.

## REPOSITORIO Y RUTA/FUNCION
- `D:\imagenesda\GestorDocumental\DocuArchiCore\MiApp.Repository\Repositorio\Workflow\RutaTrabajo\RegistroRadicadoTareaWorkflowRepository.cs`
  - `RegistrarTareaWorkflowAsync(...)`

## REGLAS
- Mantener contrato de respuesta y semantica de errores.
- No alterar logica de negocio del workflow.
- Migracion controlada con validacion inmediata.

## VALIDACION
- Build de proyectos impactados.
- Tests focalizados de Workflow.
- Revisar riesgos de regresion funcional.

## ENTREGABLE
- funciones migradas
- funciones pendientes
- decisiones tecnicas
- plan de cierre
