# SCRUM-207 - Pruebas API Gabinete Workflow

## 1. Unitarias (Controller)

Archivo: `tests/TramiteDiasVencimiento.Tests/SolicitaGabineteRadicadoWorkflowControllerContractTests.cs`

Casos cubiertos:
1. Claim válido + service OK por radicado -> `200`.
2. Claim workflow faltante -> `400`.

## 2. Unitarias (Service)

Archivo: `tests/TramiteDiasVencimiento.Tests/SolicitaGabineteRadicadoWorkflowServiceTests.cs`

Casos cubiertos:
1. Flujo exitoso por radicado usando alias workflow para gabinete.
2. `idTareaWorkflow` inválido (`<= 0`) -> validación.
3. Ruta activa no encontrada -> error controlado.
4. `Nombre_Ruta` inválido por regex -> validación.

## 3. Unitarias (Repository)

Archivo: `tests/TramiteDiasVencimiento.Tests/SolicitaGabineteRadicadoWorkflowRepositoryTests.cs`

Casos cubiertos:
1. Alias workflow vacío -> validación.
2. Radicado no encontrado -> `YES` con `EstadoExistenciaRadicado=NO`.
3. Consulta por tarea + resolución de gabinete -> `YES` con `NombreGabinete`.
4. Si no se resuelve `NombreGabinete` con `EstadoExistenciaRadicado=YES` -> `success=false` (`NombreGabinete requerido`).

## 4. Ejecución realizada

Comando:
```bash
dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter "SolicitaGabineteRadicadoWorkflow"
```

Resultado:
1. Ejecución actual en este workspace: bloqueada por lock de compilación `CS2012` en `MiApp.DTOs.dll`.
2. Validar nuevamente cuando se libere el archivo en uso.

## 5. Pruebas recomendadas (integración manual)
1. Llamar endpoint por radicado con registro existente.
2. Llamar endpoint por radicado sin registro.
3. Llamar endpoint por tarea con registro existente.
4. Ejecutar con JWT sin `defaulaliaswf` y validar `400`.
5. Validar `NombreGabinete` para `id_Gabinete` existente en `configuracion_gabinete`.
