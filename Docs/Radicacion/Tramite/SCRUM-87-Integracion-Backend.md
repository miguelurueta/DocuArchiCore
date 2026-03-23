# SCRUM-87 Integracion Backend

## Objetivo

Integrar `SolicitaExistenciaRadicadoRutaWorkflowAsync` dentro del flujo de `RegistrarRadicacionEntranteAsync` sin modificar el contrato de respuesta del endpoint.

## Flujo actualizado

1. `RegistrarRadicacionEntranteAsync` valida request, usuario, plantilla y workflow inicial.
2. Si `tipoModuloRadicacion = 2`, ejecuta `ValidaPreRegistroWorkflowAsync`.
3. El pre-registro devuelve `NombreRuta` como sufijo real de la tabla workflow.
4. El servicio registra el radicado mediante `_registrarRepository.RegistrarRadicacionEntranteAsync`.
5. Inmediatamente después, consulta `SolicitaExistenciaRadicadoRutaWorkflowAsync` usando:
   - `nombreRuta = workflowValidation.NombreRuta`
   - `consecutivoRadicado = ReturnRegistraRadicacion.ConsecutivoRadicado` o fallback legado
   - `defaultDbAlias = claim defaulaliaswf`
6. El resultado de la consulta se mantiene en una variable interna del flujo y no se expone en el DTO público.

## Reglas funcionales

- La consulta de existencia workflow solo corre cuando aplica módulo workflow.
- Si la consulta workflow falla, el servicio retorna `success = false` con el error controlado.
- Si la consulta workflow responde correctamente, el flujo continúa sin ampliar el contrato de salida.

## Evidencia de pruebas

- Se agregó cobertura unitaria para confirmar que:
  - el flujo workflow llama al servicio de existencia con `NombreRuta` y `defaulaliaswf`
  - el flujo retorna error controlado cuando la consulta workflow falla
