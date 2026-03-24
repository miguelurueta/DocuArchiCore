# SCRUM-92 - Integracion Backend

## Resumen

- `ValidaPreRegistroWorkflowAsync` ahora retorna `RutaWorkflow` junto con `NombreRuta` y las relaciones mapeadas.
- `RegistrarRadicacionEntranteAsync` conserva el resultado de pre-registro workflow y bloquea el registro cuando `tipoModuloRadicacion = 2` no recibe una ruta valida.
- Para `tipoModuloRadicacion != 2`, el flujo valida primero la ruta workflow interna y luego el `UsuarioWorkflow` interno antes de llamar al repositorio de registro.

## Repos Impactados

- `DocuArchiCore`: OpenSpec, pruebas y evidencia tecnica.
- `MiApp.DTOs`: contrato `ValidaPreRegistroWorkflowResultDto`.
- `MiApp.Services`: orquestacion `RegistrarRadicacionEntranteService` y retorno de `ValidaPreRegistroWorkflowService`.

## Flujo Ajustado

1. `RegistrarRadicacionEntranteAsync` ejecuta validaciones base de request, plantilla y configuracion.
2. Si `tipoModuloRadicacion = 2`, ejecuta `ValidarPreRegistroWorkflowAsync` y exige `data.RutaWorkflow` valida antes de registrar.
3. Si `tipoModuloRadicacion != 2`, consulta ruta workflow interna valida y luego `UsuarioWorkflow` interno valido.
4. Solo despues de esas validaciones ejecuta `_registrarRepository.RegistrarRadicacionEntranteAsync`.
5. Si el registro workflow fue exitoso, consulta la existencia del radicado en la ruta workflow con `NombreRuta`.

## Cobertura Esperada

- Falla controlada cuando el pre-registro workflow no retorna `RutaWorkflow`.
- Falla controlada cuando no existe ruta workflow interna para el flujo no-workflow.
- Flujo exitoso cuando existe `RutaWorkflow` valida y no se consulta `UsuarioWorkflow` interno en `tipoModuloRadicacion = 2`.
- Compatibilidad con la normalizacion legacy de `ReturnRegistraRadicacion`.

## Verificacion

- Pendiente ejecutar `dotnet test` contra un harness que apunte a los worktrees limpios de `MiApp.Services` y `MiApp.DTOs`.
- `openspec validate` pendiente al cierre del cambio.
