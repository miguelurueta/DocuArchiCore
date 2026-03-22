# SCRUM-83 Migracion Legacy

## Funcion origen

- Legacy: `Solicita_datos_actividad_inicio_flujo`
- Archivo: `workflow/Class_flujo_trabajo_workflow.vb`

## Comportamiento identificado

- Consulta la tabla `wf_registro_actividaes_flujos_trabajo`.
- Filtra por `wf_flujos_trabajo_ID_WF_FLUJOS_TRABAJO = id_flujo_trabajo`.
- Filtra por `ACTIVIDAD_INICIO = 1`.
- Retorna:
  - `ID_REGISTRO_ACTIVIDAD_FLUJO_TRABAJO`
  - `listado_actividades_workflow_Id_Actividad`
  - `ID_USUARIO_WORKFLOW`
- Si no hay fila, retorna `YES` y deja los ids en `0`.
- Si `ID_USUARIO_WORKFLOW` es `NULL`, lo normaliza a `0`.

## Decision de migracion

- Se implementa solo `Repository`, `DTO` y `Model`.
- No se crea `Service` ni `Controller` en este alcance.
- La consulta usa `QueryOptions` y `DapperCrudEngine`.
- El alias esperado para workflow es `MySqlConnection_WF`.
