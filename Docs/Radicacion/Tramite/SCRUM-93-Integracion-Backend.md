# SCRUM-93 - Integracion Backend

## Resumen

- `RegistrarRadicacionEntranteService` ahora valida la actividad workflow relacionada al grupo del `UsuarioWorkflow` antes de registrar.
- La validacion solo aplica cuando `citaEstructuraTipoDoEntrante.util_tipo_modulo_envio` es `2` o `3`.
- Si el grupo workflow no tiene `id_Actividad` valido, el flujo se interrumpe con error controlado y no ejecuta el repositorio de registro.

## Impacto Real

- `MiApp.Services`: integra `SolicitaIdActividadRelacionadaGrupo` en el flujo de registro.
- `MiApp.Repository`: sin cambio funcional nuevo; el repositorio ya existe en `main`.
- `MiApp.Models`: sin cambio funcional nuevo; `GruposWorkflow.id_Actividad` ya existe en `main`.
- `DocuArchiCore`: pruebas, spec, tasks y evidencia tecnica.

## Flujo Ajustado

1. Se resuelve `UsuarioWorkflow` interno.
2. Si `util_tipo_modulo_envio` es `2` o `3`, el servicio toma `usuarioWorkflowInterno.Grupos_Workflow_Id_Grupo`.
3. Con `defaulaliaswf` consulta `SolicitaIdActividadRelacionadaGrupo`.
4. El resultado se guarda en una variable local `GruposWorkflow`.
5. Si `GruposWorkflow` no existe o `id_Actividad <= 0`, el flujo retorna validacion controlada.
6. Solo despues de esa validacion se ejecuta `_registrarRepository.RegistrarRadicacionEntranteAsync`.

## Cobertura Esperada

- Exito cuando el grupo workflow tiene actividad relacionada valida.
- Error controlado cuando el grupo workflow no tiene `id_Actividad`.
- No se consulta el grupo workflow cuando `util_tipo_modulo_envio` no es `2` ni `3`.
