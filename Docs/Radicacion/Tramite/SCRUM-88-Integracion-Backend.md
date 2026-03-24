# SCRUM-88 Integracion Backend

## Objetivo

Integrar `SolicitaEstructuraRutaWorkflowAsync` dentro del flujo de `RegistrarRadicacionEntranteAsync` como dato interno previo al registro cuando `tipoModuloRadicacion != 2`.

## Flujo actualizado

1. `RegistrarRadicacionEntranteAsync` valida request, usuario, plantilla, configuracion y reglas base.
2. Si `tipoModuloRadicacion == 2`, el flujo mantiene la prevalidacion workflow existente.
3. Si `tipoModuloRadicacion != 2`, el servicio obtiene `defaulaliaswf` desde claims.
4. Con ese alias consulta `SolicitaEstructuraRutaWorkflowAsync` antes de `_registrarRepository.RegistrarRadicacionEntranteAsync`.
5. El resultado se usa solo como validacion interna del flujo; no se expone en el DTO publico.
6. Si no existe una ruta workflow activa valida, el servicio retorna error controlado y no ejecuta el registro.

## Reglas funcionales

- La consulta interna de estructura workflow corre solo para `tipoModuloRadicacion != 2`.
- La ruta workflow se considera valida cuando existe al menos un registro con `id_Ruta > 0`.
- Si falta el claim `defaulaliaswf`, el flujo retorna validacion controlada.
- Si la consulta retorna sin resultados, el flujo no continua con `_registrarRepository.RegistrarRadicacionEntranteAsync`.

## Evidencia de pruebas

- Se agrego cobertura unitaria para confirmar que:
  - el flujo no workflow consulta `SolicitaEstructuraRutaWorkflowAsync` antes del registro
  - el flujo retorna error cuando falta `defaulaliaswf`
  - el flujo retorna error cuando no existe estructura workflow activa valida
  - el flujo legado sigue normalizando `ReturnRegistraRadicacion`
