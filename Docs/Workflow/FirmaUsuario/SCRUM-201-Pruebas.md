# SCRUM-201 - Pruebas API Firma Temporal Usuario Workflow

## Unitarias (Controller)

Archivo: `tests/TramiteDiasVencimiento.Tests/FirmaTemporalUsuarioWorkflowControllerTests.cs`

Casos cubiertos:
- Claim `defaulaliaswf` faltante/inválido -> `400`.
- Flujo exitoso de metadata -> `200`.
- Descarga con token inexistente -> `404`.

## Unitarias (Service)

Archivo: `tests/TramiteDiasVencimiento.Tests/ServiceFirmaTemporalUsuarioWorkflowTests.cs`

Casos cubiertos:
- Alias inválido -> `validation`.
- Firma PNG válida:
  - genera archivo temporal
  - genera URL temporal
  - resuelve token para descarga (`TryResolveFirmaTemporal`).

## Ejecución realizada

Comando:
```bash
dotnet test tests/TramiteDiasVencimiento.Tests/TramiteDiasVencimiento.Tests.csproj --filter "FullyQualifiedName~FirmaTemporalUsuarioWorkflowControllerTests|FullyQualifiedName~ServiceFirmaTemporalUsuarioWorkflowTests"
```

Resultado:
- Total: 5
- Exitosas: 5
- Fallidas: 0

## Pruebas recomendadas (integración manual)
1. JWT sin `defaulaliaswf` -> validar `400`.
2. JWT con claims válidos y firma en DB -> validar metadata + descarga `200`.
3. Esperar expiración TTL -> descarga `404`.
4. Usuario sin firma -> metadata `400` con estado `validation`.
