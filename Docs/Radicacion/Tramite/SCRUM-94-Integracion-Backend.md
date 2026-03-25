# SCRUM-94 - Integracion Backend

## Resumen

- `RegistrarRadicacionEntranteService` ya no depende solo de `requestCanonico.tipoModuloRadicacion` para decidir la validacion de actividad workflow relacionada.
- El flujo ahora usa `citaEstructuraTipoDoEntrante.util_tipo_modulo_envio` para determinar cuando debe resolver `UsuarioWorkflow` y consultar `SolicitaIdActividadRelacionadaGrupo`.
- Se conserva compatibilidad para el flujo workflow que no requiere actividad relacionada por tipo de envio.

## Impacto Real

- `MiApp.Services`: ajusta la condicion de validacion de `UsuarioWorkflow` dentro de `RegistrarRadicacionEntranteAsync`.
- `DocuArchiCore`: actualiza pruebas, spec, tasks y evidencia tecnica.
- `MiApp.Repository`, `MiApp.Models`, `MiApp.DTOs`, `DocuArchi.Api`: sin cambio funcional adicional.

## Flujo Ajustado

1. El servicio obtiene `citaEstructuraTipoDoEntrante`.
2. Calcula si el tipo de envio requiere actividad relacionada (`util_tipo_modulo_envio == 2 || util_tipo_modulo_envio == 3`).
3. Si aplica, resuelve `UsuarioWorkflow` aunque `requestCanonico.tipoModuloRadicacion` sea `2`.
4. Con `usuarioWorkflowInterno.Grupos_Workflow_Id_Grupo` consulta `SolicitaIdActividadRelacionadaGrupo`.
5. Solo continua con `_registrarRepository.RegistrarRadicacionEntranteAsync` si existe actividad relacionada valida.
6. Si el tipo de envio no requiere actividad relacionada, conserva el comportamiento workflow actual.

## Cobertura Esperada

- Exito cuando el flujo workflow usa `util_tipo_modulo_envio = 2` y existe `UsuarioWorkflow` con actividad relacionada valida.
- Compatibilidad cuando el flujo workflow no requiere actividad relacionada adicional.
- Error controlado si el tipo de envio requiere actividad relacionada pero no existe `UsuarioWorkflow` o actividad valida.

## Verificacion

- `dotnet test .tmp\scrum94-harness\TramiteDiasVencimiento.Tests.csproj --filter RegistrarRadicacionEntranteServiceTests -m:1` -> `12/12` OK
- `dotnet test tests\TramiteDiasVencimiento.Tests\TramiteDiasVencimiento.Tests.csproj --filter RegistrarRadicacionEntranteServiceTests -m:1` sigue bloqueado por un problema preexistente de restore en `DocuArchi.Api` (`NU1605` con `AutoMapper`), no por el cambio de `SCRUM-94`
