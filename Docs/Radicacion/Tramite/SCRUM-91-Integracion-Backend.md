# SCRUM-91 - Integracion Backend

## Objetivo

Ajustar `ValidarUsuarioWorkflowInternoAsync` para que retorne el `UsuarioWorkflow` valido al flujo principal de `RegistrarRadicacionEntranteAsync`, con error controlado cuando el usuario workflow no exista o la consulta falle.

## Flujo actualizado

1. `RegistrarRadicacionEntranteAsync` ejecuta `ValidarUsuarioWorkflowInternoAsync` cuando `tipoModuloRadicacion != 2`.
2. `ValidarUsuarioWorkflowInternoAsync` valida destinatario interno, `Relacion_Workflow`, claim `defaulaliaswf` y consulta `SolicitaEstructuraIdUsuarioWorkflowId(...)`.
3. Si la consulta retorna un `UsuarioWorkflow` valido, el flujo principal lo conserva en una variable local.
4. Antes de registrar, el servicio valida que ese `UsuarioWorkflow` no sea `null`.
5. Si la consulta falla o lanza excepcion, el flujo retorna un `AppResponses` controlado y no invoca `_registrarRepository.RegistrarRadicacionEntranteAsync`.

## Comportamientos esperados

- El flujo no workflow sigue interrumpiendose cuando `Relacion_Workflow` es invalida.
- El flujo no workflow sigue interrumpiendose cuando falta `defaulaliaswf`.
- Una excepcion del repositorio de workflow ahora se traduce en mensaje controlado `Error al validar usuario workflow interno`.
- El servicio conserva el `UsuarioWorkflow` resuelto para uso posterior en el flujo principal.

## Evidencia

- Archivo principal: `MiApp.Services/Service/Radicacion/Tramite/RegistrarRadicacionEntranteService.cs`
- Pruebas unitarias: `DocuArchiCore/tests/TramiteDiasVencimiento.Tests/RegistrarRadicacionEntranteServiceTests.cs`
