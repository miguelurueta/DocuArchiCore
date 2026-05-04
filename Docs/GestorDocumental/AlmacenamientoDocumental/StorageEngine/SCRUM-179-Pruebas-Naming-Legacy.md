# SCRUM-179 — Pruebas Naming Legacy

## Pruebas unitarias agregadas
- `StorageNamingServiceTests`
  - Verifica construcción legacy:
    - `DIG00000001.pdf`
    - `DIG00000010.tif`
    - `DIG00000100.jpg`
    - `DIG00001000.bmp`
  - Verifica XML:
    - `FXL00000001.xml`, etc.
  - Verifica segundo nombre por defecto.

- `StorageExtensionResolver`
  - Normaliza extensión sin punto (`PDF -> .pdf`).
  - Error cuando repositorio retorna nulo.

## Pruebas ajustadas
- `StoragePhysicalPhaseExecutorTests`
  - Migradas a `BuildFilePlanAsync`.
  - Expectativas actualizadas de `alm_` a `DIG/FXL`.

## Casos recomendados para integración
- `DA_EXTENSION` sin registro para tipo documental -> error funcional.
- `DA_EXTENSION` con extensión en mayúscula -> normalización correcta.
- Escritura física y XML con nombres legacy en ruta final.

## Estado de ejecución en sesión
- No se obtuvo ejecución confiable de `dotnet test` por incidencia de entorno SDK/MSBuild (`MSB4276`).
- Resultado: validación manual de compilación lógica + trazabilidad de cambios.

