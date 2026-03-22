# SCRUM-83 Integracion Backend

## Rutas destino aplicadas

- `MiApp.Repository/Repositorio/Workflow/Flujo`
- `MiApp.DTOs/DTOs/Workflow/Flujo`
- `MiApp.Models/Models/Workflow/Flujo`
- `DocuArchiCore/Docs/Workflow/Flujo`

## Contrato implementado

```csharp
Task<AppResponses<SolicitaDatosActividadInicioFlujo>> SolicitaDatosActividadInicioFlujoAsync(
    int idFlujoTrabajo,
    string defaultDbAlias)
```

## Respuesta funcional

- `success = true`, `message = "YES"` cuando existe registro.
- `success = true`, `message = "Sin resultados"` cuando no existe registro.
- `success = false` cuando la entrada es invalida o falla la consulta.

## Evidencia

- `dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter FullyQualifiedName~SolicitaDatosActividadInicioFlujoRepositoryTests`
- Resultado: `4` pruebas exitosas, `0` fallidas, `0` omitidas.
