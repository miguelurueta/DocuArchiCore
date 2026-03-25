# SCRUM-96 - Integracion Backend

## Resumen

- `RegistrarRadicacionEntranteService` mueve la variable `existenciaWorkflowResult` fuera del bloque `if (registroResult.success && registroResult.data != null)`.
- La consulta `ConsultarExistenciaRadicadoRutaWorkflowAsync(...)` solo se ejecuta cuando el registro fue exitoso y retornó `data`, pero el manejo del resultado queda fuera de esa condicion.
- El flujo mantiene el mismo comportamiento funcional: si `existenciaWorkflowResult` falla, retorna error controlado; si es valido o no aplica, retorna `registroResult`.

## Impacto Real

- `MiApp.Services`: ajusta el bloque final de `RegistrarRadicacionEntranteAsync`.
- `DocuArchiCore`: actualiza pruebas, spec, tasks, sync y evidencia tecnica.
- `DocuArchi.Api`, `MiApp.DTOs`, `MiApp.Repository`, `MiApp.Models`: sin cambio funcional adicional.

## Flujo Ajustado

1. El servicio registra la radicacion con `_registrarRepository.RegistrarRadicacionEntranteAsync(...)`.
2. Normaliza `ReturnRegistraRadicacion` legacy si aplica.
3. Declara `existenciaWorkflowResult` fuera del `if`.
4. Solo consulta `ConsultarExistenciaRadicadoRutaWorkflowAsync(...)` cuando `registroResult.success` y `registroResult.data` son validos.
5. Evalua `existenciaWorkflowResult` fuera del bloque y retorna error controlado si `success=false`.
6. Si no hay error, retorna `registroResult`.

## Cobertura Esperada

- Exito cuando el registro termina correctamente y la consulta de existencia workflow es valida.
- Error controlado cuando la consulta de existencia workflow falla despues de un registro exitoso.
- Compatibilidad con la normalizacion legacy de `ReturnRegistraRadicacion`.

## Verificacion

- `dotnet test .tmp\scrum95-harness\TramiteDiasVencimiento.Tests.csproj --filter RegistrarRadicacionEntranteServiceTests -m:1` -> `14/14` OK
