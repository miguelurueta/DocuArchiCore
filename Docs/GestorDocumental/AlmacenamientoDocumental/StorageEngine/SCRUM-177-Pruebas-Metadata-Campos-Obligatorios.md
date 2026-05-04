# SCRUM-177 Pruebas Metadata Campos Obligatorios

## Matriz de pruebas unitarias objetivo

| Caso | Esperado | Estado |
|---|---|---|
| Metadata no existe | `GAB_FIELDS_NOT_FOUND` | Cubierto (unit test actualizado) |
| Cantidad metadata != cantidad request | `GAB_FIELDS_MISMATCH` | Cubierto (unit test actualizado) |
| Campo obligatorio vacio | `GAB_REQUIRED_EMPTY` | Cubierto (unit test nuevo) |
| Desalineacion de campo | `GAB_FIELD_UNKNOWN` | Cubierto por mapeo de excepcion |
| Validacion OK | Sin errores en validator | Cubierto por flujo existente del pipeline |

## Pruebas tocadas
- `tests/TramiteDiasVencimiento.Tests/StorageValidationPipelineTests.cs`
  - actualiza mocks de `IStorageGabineteMetadataProvider` a `GetMetadataAsync`.
  - incorpora `IStorageRequiredFieldsValidator` en constructor de `GabineteRequiredFieldsValidator`.
  - agrega caso `GAB_REQUIRED_EMPTY`.

## Ejecucion local
- `dotnet build ..\MiApp.Models\MiApp.Models.csproj -c Debug`: OK (con warnings legacy preexistentes).
- `dotnet build ..\MiApp.Repository\MiApp.Repository.csproj`: bloqueado por entorno SDK/MSBuild (sin errores de codigo reportados).
- `dotnet build ..\MiApp.Services\MiApp.Services.csproj`: bloqueado por entorno SDK/MSBuild (sin errores de codigo reportados).
- `dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter FullyQualifiedName~StorageValidationPipelineTests`: bloqueado por el mismo problema de entorno.

## Riesgos residuales
- No hay validacion automatica de integracion DB en este corte.
- Se recomienda corrida en pipeline CI del repositorio satelite para validar restore/build/test completo de `MiApp.Repository` y `MiApp.Services`.
