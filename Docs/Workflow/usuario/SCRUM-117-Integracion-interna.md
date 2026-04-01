# SCRUM-117 - Integracion interna

## Objetivo

Exponer un repository interno para consultar la configuracion de listado del usuario workflow desde la tabla `configuracion_usuario`.

## Contrato de respuesta

Tipo de retorno:

```csharp
Task<AppResponses<configuracionUsuarioDTO?>>
```

Modelo consultado:

- `id_config`
- `Usuario_Workflow_idU_suario`
- `Numero_Tarea_Lista`
- `Fecha_ini_Lista`
- `Fecha_Fin_Lista`

## Parametros

- `idUsuarioWorkflow`: identificador del usuario workflow.
- `defaultDbAlias`: alias de base de datos workflow.

## Regla funcional

- filtro obligatorio: `Usuario_Workflow_idU_suario = idUsuarioWorkflow`
- si no hay registros: `success = true`, `data = null`, `message = "Sin resultados"`

## Ejemplo de invocacion

```csharp
var result = await repository.SolicitaConfiguracionListaUsuarioWorkflowAsync(144, "WF");
```
