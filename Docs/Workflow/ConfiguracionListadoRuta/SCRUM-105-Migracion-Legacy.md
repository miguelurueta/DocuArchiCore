# SCRUM-105 Migracion Legacy

## Funcion origen

- Legacy: `Solicita_campos_lista_tramite`
- Archivo: `GestionDocumental-Docuarchi.net/workflow/Class_configuracion_listado_ruta.vb`

## Comportamiento identificado

- Consulta la tabla `configuracion_listado_ruta`.
- Filtra por `Rutas_Workflow_id_Ruta = id_ruta_workflow`.
- Filtra por `Lista_gestion_tamite = 1`.
- Ordena por `Orden_lista_gestion_tamite`.
- Solo proyecta `Nombre_Campo`.
- Si hay registros, concatena los nombres en una cadena CSV en el mismo orden.
- Si la consulta falla, retorna un mensaje con prefijo `Error funcion Solicita_campos_lista_tramite`.
- Si ocurre excepción, retorna un mensaje con prefijo `Inconsistencia Solicita_campos_lista_tramite`.

## Implementacion .NET

- Repository: `MiApp.Repository/Repositorio/Workflow/RutaTrabajo/SolicitaCamposListaGestionCorrespondenciaRepository.cs`
- Registro DI: `DocuArchi.Api/Program.cs`
- Pruebas unitarias: `DocuArchiCore/tests/TramiteDiasVencimiento.Tests/SolicitaCamposListaGestionCorrespondenciaRepositoryTests.cs`

## Decision de migracion

- Se implementa solo `Repository`; no se crea `Service` ni `Controller` porque el alcance es una consulta directa.
- Se reutiliza el model existente `ConfiguracionListadoRuta`.
- No se crea DTO adicional porque el contrato final es `AppResponses<string?>`.
- La consulta usa `QueryOptions` con filtros parametrizados y proyeccion de columnas.

## Diferencias controladas respecto al legacy

- El legacy retornaba el mensaje textual `No hay campos activos para la lista de tramites pendientes, contacte a su adminstrador` cuando no habia filas.
- En la migracion se normaliza a `success = true`, `data = null`, `message = "Sin resultados"` para mantener consistencia con el contrato estandar usado por los repositories del proyecto.
