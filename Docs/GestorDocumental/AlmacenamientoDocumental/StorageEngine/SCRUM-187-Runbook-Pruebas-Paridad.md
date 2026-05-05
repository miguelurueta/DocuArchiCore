# SCRUM-187 — Runbook de Pruebas de Paridad

## Prerrequisitos
- .NET SDK compatible con la solución.
- Variables de conexión/alias para pruebas de integración (si aplica).
- Docker activo para pruebas Testcontainers.

## Ejecución recomendada
1. Ejecutar suite de paridad local:
```powershell
dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter "FullyQualifiedName~StorageEngineParity"
```
2. Ejecutar regresión de componentes storage relevantes:
```powershell
dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter "FullyQualifiedName~StorageTransactionCoordinator|FullyQualifiedName~WorkflowStorageLog|FullyQualifiedName~StorageOptionsResolver|FullyQualifiedName~ExpedienteIndiceXml"
```
3. En pipeline con Docker, habilitar corrida de pruebas `Skip` (remover skip temporalmente o activar variante de integración).

## Validaciones mínimas de salida
- Todos los tests activos en verde.
- Sin colisiones de identidad en concurrencia mínima.
- XML FXL contiene atributos legacy críticos.
- Matriz de paridad actualizada.
