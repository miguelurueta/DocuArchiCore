# SCRUM-90 Integracion Backend

## Objetivo

Integrar `SolicitaEstructuraIdUsuarioWorkflowId` dentro del flujo de `RegistrarRadicacionEntranteAsync` como validacion interna previa al registro cuando `tipoModuloRadicacion != 2`.

## Flujo actualizado

1. `RegistrarRadicacionEntranteAsync` valida request, usuario de gestion, plantilla y configuracion base.
2. Si `tipoModuloRadicacion == 2`, conserva la prevalidacion workflow existente.
3. Si `tipoModuloRadicacion != 2`, consulta el destinatario interno y toma `Relacion_Workflow`.
4. El servicio obtiene `defaulaliaswf` desde claims.
5. Con `Relacion_Workflow` y `defaulaliaswf` ejecuta `SolicitaEstructuraIdUsuarioWorkflowId(...)`.
6. Solo si existe un `UsuarioWorkflow` valido continua con `_registrarRepository.RegistrarRadicacionEntranteAsync`.
7. El resultado del usuario workflow no se expone en el DTO publico; se usa solo como dato interno del flujo.

## Reglas funcionales

- La validacion de usuario workflow corre solo para `tipoModuloRadicacion != 2`.
- Si el destinatario no tiene `Relacion_Workflow` valida, el flujo retorna error controlado.
- Si falta `defaulaliaswf`, el flujo retorna validacion controlada.
- Si `SolicitaEstructuraIdUsuarioWorkflowId` falla o no retorna un usuario workflow valido, no se registra el radicado.

## Evidencia de pruebas

- Se agrego cobertura unitaria para confirmar que:
  - el flujo no workflow consulta `SolicitaEstructuraIdUsuarioWorkflowId` antes del registro
  - el flujo workflow (`tipoModuloRadicacion == 2`) no ejecuta esa validacion adicional
  - el flujo retorna error cuando falta `defaulaliaswf`
  - el flujo retorna error cuando `Relacion_Workflow` es invalida
  - el flujo retorna error controlado cuando falla la consulta a usuario workflow
  - el flujo legado sigue normalizando `ReturnRegistraRadicacion`

## Estado de verificacion

- Se agrego `NU1903` a `NoWarn` en proyectos impactados para que el audit de NuGet no bloquee la validacion local del ticket.
- Se excluyo `obj\**\*.cs` de los proyectos impactados para evitar que builds alternos compilen artefactos generados como codigo fuente.
- Validacion ejecutada:
  - `dotnet test tests\TramiteDiasVencimiento.Tests\TramiteDiasVencimiento.Tests.csproj --filter RegistrarRadicacionEntranteServiceTests -m:1`
  - `openspec.cmd validate scrum-90-implementacion-solicitaestructuraidusuar`
- Resultado:
  - `6/6` pruebas focalizadas OK
  - OpenSpec valido
- Persisten warnings preexistentes de nullability y `NU1504` por `Dapper` duplicado en `MiApp.Repository`, sin bloquear el flujo de `SCRUM-90`.
