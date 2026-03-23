# SCRUM-84 - Integracion Backend

## Objetivo

Integrar `SolicitaDatosActividadInicioFlujoAsync` dentro de `RegistrarRadicacionEntranteAsync` cuando la radicacion opere en modo workflow y el request incluya `RE_flujo_trabajo.id_tipo_flujo_workflow`.

## Cambios aplicados

- `RegistrarRadicacionEntranteService` ahora consulta la actividad inicial workflow antes de continuar con el preregistro y el registro definitivo.
- El flujo corta con respuesta controlada si `defaulaliaswf` no existe o si la consulta no devuelve actividad inicial valida.
- `DocuArchi.Api/Program.cs` registra `ISolicitaDatosActividadInicioFlujoRepository` para resolver la nueva dependencia en DI.
- Se ampliaron las pruebas unitarias del servicio para cubrir:
  - solicitud no workflow: no consulta actividad inicial
  - workflow sin actividad inicial: detiene el registro
  - workflow con actividad inicial valida: continua y registra

## Comportamiento funcional

- Si `tipoModuloRadicacion != 2`, el flujo no consulta `SolicitaDatosActividadInicioFlujoAsync`.
- Si `tipoModuloRadicacion = 2` y `id_tipo_flujo_workflow > 0`, la consulta workflow se ejecuta antes de `ValidaPreRegistroWorkflowAsync`.
- Si la consulta retorna ids de actividad en `0`, la operacion responde `success=false` y no llama al repository de registro.
- Si la actividad inicial existe, el flujo continua con la validacion workflow actual y luego con el registro.

## Evidencia tecnica

- `dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter FullyQualifiedName~RegistrarRadicacionEntranteServiceTests`
- Resultado: `10` pruebas exitosas, `0` fallidas, `0` omitidas.
- `dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter "FullyQualifiedName~SolicitaDatosActividadInicioFlujoRepositoryTests|FullyQualifiedName~RadicacionControllerContractTests"`
- Resultado: `12` pruebas exitosas, `0` fallidas, `0` omitidas.
