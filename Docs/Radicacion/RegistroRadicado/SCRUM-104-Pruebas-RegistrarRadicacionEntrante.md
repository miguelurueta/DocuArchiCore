# SCRUM-104 - Pruebas RegistrarRadicacionEntranteAsync

## Resumen

- El cambio se enfoca en ampliar la cobertura unitaria de `RegistrarRadicacionEntranteAsync`.
- No se introduce nueva logica funcional en el servicio; se protege el comportamiento actual con pruebas deterministas.
- La evidencia se concentra en validaciones de entrada, preregistro workflow y errores controlados al registrar tarea workflow.

## Cobertura Implementada

- `request = null` retorna error de validacion controlado y no ejecuta el repositorio de registro.
- `ValidaPreRegistroWorkflowAsync(...)` con `success = false` detiene el flujo y conserva el error controlado.
- Cuando no existe tarea workflow y el repositorio `IRegistroRadicadoTareaWorkflowRepository` no esta registrado, el servicio retorna error de dependencia controlado.
- Cuando falta el claim `defaulaliaswf` durante el registro de tarea workflow, el servicio retorna validacion controlada y no ejecuta `RegistrarTareaWorkflowAsync(...)`.
- Se mantiene la cobertura existente para escenarios de exito, consultas workflow sin datos, usuario workflow, actividad relacionada y actualizacion de estado.

## Archivos Impactados

- `tests/TramiteDiasVencimiento.Tests/RegistrarRadicacionEntranteServiceTests.cs`
- `openspec/changes/scrum-104-test-servicio-registrarradicacionentrant/specs/jira-scrum-104/spec.md`
- `openspec/changes/scrum-104-test-servicio-registrarradicacionentrant/tasks.md`

## Verificacion

- Comando ejecutado:
  - `dotnet test tests\\TramiteDiasVencimiento.Tests\\TramiteDiasVencimiento.Tests.csproj --filter RegistrarRadicacionEntranteServiceTests -m:1`

- Resultado:
  - `23/23` pruebas correctas en `RegistrarRadicacionEntranteServiceTests`.
  - La ejecucion mantiene advertencias preexistentes de restore/vulnerabilidad (`NU1900`) por acceso al feed, pero no hubo fallas de compilacion ni de pruebas en la suite focalizada.

## Limitaciones Conocidas

- Este cambio no agrega pruebas de integracion reales con MySQL/Testcontainers.
- Si se requiere cobertura de infraestructura real, debe abordarse en un ticket especifico con schema/seed controlados y disponibilidad de Docker.
