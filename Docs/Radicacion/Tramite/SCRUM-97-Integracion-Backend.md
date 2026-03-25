# SCRUM-97 - Integracion Backend

## Resumen

- `RegistrarRadicacionEntranteService` ajusta el flujo final de `ConsultarExistenciaRadicadoRutaWorkflowAsync(...)`.
- Cuando la consulta de existencia workflow retorna `success=true` con `data=null`, el servicio ya no cancela la ejecucion.
- `existenciaWorkflowResult` queda declarado fuera del bloque `if (registroResult.success && registroResult.data != null)` para permitir reutilizacion posterior, usando una respuesta controlada por defecto y no una inicializacion nula.

## Impacto Real

- `MiApp.Services`: ajusta el bloque final de `RegistrarRadicacionEntranteAsync` y normaliza respuestas exitosas sin datos desde `ConsultarExistenciaRadicadoRutaWorkflowAsync`.
- `DocuArchiCore`: actualiza pruebas, spec, tasks, sync y evidencia tecnica.
- `DocuArchi.Api`, `MiApp.DTOs`, `MiApp.Repository`, `MiApp.Models`: sin cambio funcional adicional.

## Flujo Ajustado

1. El servicio registra la radicacion con `_registrarRepository.RegistrarRadicacionEntranteAsync(...)`.
2. `existenciaWorkflowResult` se declara antes de evaluar el bloque final del registro.
3. Si el registro fue exitoso y trae `data`, consulta `ConsultarExistenciaRadicadoRutaWorkflowAsync(...)`.
4. Si esa consulta retorna `success=false`, el flujo responde con error controlado.
5. Si esa consulta retorna `success=true` con `data=null`, el flujo continua sin cancelar el registro.
6. El resultado queda disponible para reutilizacion posterior dentro del metodo.

## Cobertura Esperada

- Exito cuando la consulta de existencia workflow retorna datos validos.
- Exito cuando la consulta de existencia workflow retorna `success=true` sin datos.
- Error controlado cuando la consulta de existencia workflow falla.

## Verificacion

- `dotnet test .tmp\scrum95-harness\TramiteDiasVencimiento.Tests.csproj --filter RegistrarRadicacionEntranteServiceTests -m:1` -> `15/15` OK
