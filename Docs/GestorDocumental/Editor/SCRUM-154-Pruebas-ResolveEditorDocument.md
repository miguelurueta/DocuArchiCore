# SCRUM-154 — Pruebas: ResolveEditorDocument

## Unitarias (Controller)

- `Resolve_CuandoClaimInvalido_RetornaBadRequest`
- `Resolve_CuandoParametrosInvalidos_RetornaBadRequest`
- `Resolve_CuandoPreferInvalido_RetornaBadRequest`
- `Resolve_CuandoServiceConflict_RetornaConflict`
- `Resolve_CuandoServiceOk_RetornaOk`

Archivo:
- `tests/TramiteDiasVencimiento.Tests/ResolveEditorDocumentControllerTests.cs`

## Integración / QT

Pendiente: ejecutar `dotnet test` en ambiente con restore/herramientas habilitadas.

Nota: en este entorno se observan restricciones de ejecución de scripts (PowerShell profile no firmado) que pueden bloquear la ejecución automatizada. Documentar evidencia al correrlo en CI/ambiente del equipo.

