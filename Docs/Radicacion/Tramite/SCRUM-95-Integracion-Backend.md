# SCRUM-95 - Integracion Backend

## Resumen

- `RegistrarRadicacionEntranteService` reemplaza las condiciones workflow restantes basadas en `requestCanonico.tipoModuloRadicacion == 2`.
- El flujo ahora decide preregistro, validacion de ruta workflow, actividad inicial y consulta de existencia usando `citaEstructuraTipoDoEntrante.util_tipo_modulo_envio`.
- Se mantiene la integracion previa de `UsuarioWorkflow` y actividad relacionada al grupo, pero ahora consistente con el mismo criterio de tipo de envio.

## Impacto Real

- `MiApp.Services`: ajusta las condiciones workflow restantes dentro de `RegistrarRadicacionEntranteAsync`.
- `DocuArchiCore`: actualiza pruebas, spec, tasks y evidencia tecnica.
- `DocuArchi.Api`, `MiApp.DTOs`, `MiApp.Repository`, `MiApp.Models`: sin cambio funcional adicional.

## Flujo Ajustado

1. El servicio obtiene `citaEstructuraTipoDoEntrante`.
2. Calcula si el tipo de envio workflow aplica con `util_tipo_modulo_envio == 2 || util_tipo_modulo_envio == 3`.
3. Si aplica:
   - exige actividad inicial cuando el request trae `id_tipo_flujo_workflow`
   - ejecuta preregistro workflow
   - valida `RutaWorkflow`
   - consulta existencia workflow despues del registro
4. Si no aplica:
   - omite preregistro y existencia workflow
   - conserva la validacion de ruta interna para el flujo no workflow

## Cobertura Esperada

- Exito cuando `tipoModuloRadicacion = 2` pero `util_tipo_modulo_envio` no requiere workflow y el flujo usa ruta interna.
- Error controlado cuando `util_tipo_modulo_envio = 2/3` y el preregistro no retorna ruta valida.
- Error controlado cuando `util_tipo_modulo_envio = 2/3` exige actividad inicial y no existe configuracion valida.

## Verificacion

- `dotnet test .tmp\scrum95-harness\TramiteDiasVencimiento.Tests.csproj --filter RegistrarRadicacionEntranteServiceTests -m:1` -> `13/13` OK
- `openspec.cmd validate scrum-95-actualizacion-registrarradicacionentrant`
