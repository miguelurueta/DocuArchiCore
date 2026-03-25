# SCRUM-97 - Integracion Backend

## Resumen

- `RegistrarRadicacionEntranteService` ajusta el flujo final de `ConsultarExistenciaRadicadoRutaWorkflowAsync(...)`.
- Cuando la consulta de existencia workflow retorna `success=true` con `data=null`, el servicio ya no cancela la ejecucion.
- Se elimina el patron basado en `AppResponses<SolicitaExistenciaRadicadoRutaWorkflowDto>? existenciaWorkflowResult = null`.

## Impacto Real

- `MiApp.Services`: ajusta el bloque final de `RegistrarRadicacionEntranteAsync` y normaliza respuestas exitosas sin datos desde `ConsultarExistenciaRadicadoRutaWorkflowAsync`.
- `DocuArchiCore`: actualiza pruebas, spec, tasks, sync y evidencia tecnica.
- `DocuArchi.Api`, `MiApp.DTOs`, `MiApp.Repository`, `MiApp.Models`: sin cambio funcional adicional.

## Flujo Ajustado

1. El servicio registra la radicacion con `_registrarRepository.RegistrarRadicacionEntranteAsync(...)`.
2. Si el registro fue exitoso y trae `data`, consulta `ConsultarExistenciaRadicadoRutaWorkflowAsync(...)`.
3. Si esa consulta retorna `success=false`, el flujo responde con error controlado.
4. Si esa consulta retorna `success=true` con `data=null`, el flujo continua sin cancelar el registro.
5. El servicio retorna `registroResult` cuando no existe error tecnico en la consulta workflow.

## Cobertura Esperada

- Exito cuando la consulta de existencia workflow retorna datos validos.
- Exito cuando la consulta de existencia workflow retorna `success=true` sin datos.
- Error controlado cuando la consulta de existencia workflow falla.

## Verificacion

- `dotnet test .tmp\scrum95-harness\TramiteDiasVencimiento.Tests.csproj --filter RegistrarRadicacionEntranteServiceTests -m:1` -> `15/15` OK
