# SCRUM-180 — Pruebas Ruta Física Legacy

## Pruebas Unitarias Nuevas
- `StorageFolderLegacyPolicyTests`
  - Ruta válida crea `gabinete+disco+carpeta`.
  - `disco <= 0` -> excepción.
  - `carpeta <= 0` -> excepción.
- `StoragePhysicalPathServiceTests`
  - Ruta DB nula -> error controlado.
  - Ruta final fuera de root -> bloqueado por hardening.
  - Ruta válida -> `StoragePhysicalPathModel` correcto.
- `StorageRouteRepositoryTests`
  - Verifica consulta sobre `system1rut`.
  - Verifica filtros legacy (`gabinete`, `tipo_rut`, `est_rut`).
  - Sin filas -> `null`.
- `StoragePathResolverTests`
  - `ResolveSafePath` bloquea traversal.
  - `ResolveSafePath` retorna ruta segura.

## Validación Técnica Ejecutada
- `dotnet build` exitoso en:
  - `MiApp.Models`
  - `MiApp.Repository`
  - `MiApp.Services`
  - `DocuArchi.Api`

## Estado del Proyecto de Pruebas Global
El proyecto `TramiteDiasVencimiento.Tests` tiene errores preexistentes no relacionados con SCRUM-180 (constructores/contratos en otros módulos), por lo que la corrida global de `dotnet test` no cierra en verde actualmente.

## Recomendación
Resolver el baseline del proyecto de pruebas global para habilitar ejecución total en CI de la suite.
